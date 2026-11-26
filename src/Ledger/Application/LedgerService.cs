using SystemDesign.Payments.Ledger.Domain;

namespace SystemDesign.Payments.Ledger.Application;

public sealed class LedgerService(ILedgerStore store)
{
    public Journal PostJournal(PostJournalRequest request, string correlationId)
    {
        var existing = store.FindJournalByKey(request.JournalKey);
        if (existing is not null)
            return existing;

        var legs = request.Legs
            .Select(l => new JournalLeg(l.AccountId, l.Debit, l.Credit))
            .ToArray();

        var journal = Journal.CreateBalanced(
            journalKey: request.JournalKey,
            correlationId: correlationId,
            currency: request.Currency,
            kind: JournalKind.Post,
            legs: legs,
            paymentId: request.PaymentId);

        store.AppendJournal(journal);
        return journal;
    }

    public Hold PlaceHold(PlaceHoldRequest request, string correlationId)
    {
        var existing = store.FindHoldByKey(request.HoldKey);
        if (existing is not null)
            return existing;

        var amount = new Money(request.Amount, request.Currency).EnsurePositive();
        if (store.GetAccount(request.AccountId) is null)
            throw new LedgerDomainException($"Unknown account: {request.AccountId}");
        if (store.GetAccount(request.ContraAccountId) is null)
            throw new LedgerDomainException($"Unknown contra account: {request.ContraAccountId}");

        var expiresIn = request.ExpiresInSeconds <= 0 ? 300 : request.ExpiresInSeconds;
        var hold = new Hold
        {
            HoldId = Guid.NewGuid().ToString("N"),
            HoldKey = request.HoldKey,
            AccountId = request.AccountId,
            ContraAccountId = request.ContraAccountId,
            Amount = amount,
            PaymentId = request.PaymentId,
            CorrelationId = correlationId,
            PlacedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn),
            Status = HoldStatus.Active
        };

        // Move available → held (balanced).
        var placeJournal = Journal.CreateBalanced(
            journalKey: $"hold-place:{request.HoldKey}",
            correlationId: correlationId,
            currency: amount.Currency,
            kind: JournalKind.HoldPlace,
            legs:
            [
                new JournalLeg(request.AccountId, amount.Amount, 0),
                new JournalLeg(request.ContraAccountId, 0, amount.Amount)
            ],
            paymentId: request.PaymentId,
            holdId: hold.HoldId);

        store.AppendJournal(placeJournal);
        hold.PlaceJournalId = placeJournal.JournalId;
        store.UpsertHold(hold);
        return hold;
    }

    public Hold ReleaseHold(string holdId, string correlationId, string? releaseKey = null)
    {
        var hold = store.GetHold(holdId)
            ?? throw new LedgerDomainException($"Unknown hold: {holdId}");

        if (hold.Status is HoldStatus.Released or HoldStatus.Expired or HoldStatus.Captured)
            return hold;

        var key = releaseKey ?? $"hold-release:{hold.HoldKey}";
        var existingJournal = store.FindJournalByKey(key);
        if (existingJournal is not null)
        {
            hold.Status = HoldStatus.Released;
            hold.ReleaseJournalId = existingJournal.JournalId;
            store.UpsertHold(hold);
            return hold;
        }

        // Reverse the place: held → available.
        var releaseJournal = Journal.CreateBalanced(
            journalKey: key,
            correlationId: correlationId,
            currency: hold.Amount.Currency,
            kind: JournalKind.HoldRelease,
            legs:
            [
                new JournalLeg(hold.ContraAccountId, hold.Amount.Amount, 0),
                new JournalLeg(hold.AccountId, 0, hold.Amount.Amount)
            ],
            paymentId: hold.PaymentId,
            holdId: hold.HoldId);

        store.AppendJournal(releaseJournal);
        hold.Status = HoldStatus.Released;
        hold.ReleaseJournalId = releaseJournal.JournalId;
        store.UpsertHold(hold);
        return hold;
    }

    public Journal ReverseJournal(string journalId, ReverseJournalRequest request, string correlationId)
    {
        var existing = store.FindJournalByKey(request.ReverseJournalKey);
        if (existing is not null)
            return existing;

        if (request.Percent is <= 0 or > 100)
            throw new LedgerDomainException("Percent must be in (0, 100].");

        var original = store.GetJournal(journalId)
            ?? throw new LedgerDomainException($"Unknown journal: {journalId}");

        var factor = request.Percent / 100m;
        var legs = original.Legs
            .Select(l => new JournalLeg(
                l.AccountId,
                RoundMoney(l.Credit * factor), // swap + scale
                RoundMoney(l.Debit * factor)))
            .ToArray();

        // Fix rounding so legs still balance.
        var debit = legs.Sum(l => l.Debit);
        var credit = legs.Sum(l => l.Credit);
        if (debit != credit)
        {
            var diff = debit - credit;
            var adjustable = legs.ToList();
            var idx = adjustable.FindIndex(l => l.Credit > 0);
            if (idx < 0)
                idx = 0;
            var target = adjustable[idx];
            adjustable[idx] = target with { Credit = target.Credit + diff };
            legs = adjustable.ToArray();
        }

        var reverse = Journal.CreateBalanced(
            journalKey: request.ReverseJournalKey,
            correlationId: correlationId,
            currency: original.Currency,
            kind: JournalKind.Reverse,
            legs: legs,
            paymentId: request.PaymentId ?? original.PaymentId,
            reversesJournalId: original.JournalId);

        store.AppendJournal(reverse);
        return reverse;
    }

    public AccountBalance GetBalance(string accountId) => store.GetBalance(accountId);

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
