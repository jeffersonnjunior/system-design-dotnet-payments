using SystemDesign.Payments.Ledger.Application;
using SystemDesign.Payments.Ledger.Domain;
using SystemDesign.Payments.Ledger.Infrastructure;

namespace SystemDesign.Payments.Ledger.Tests;

public class HoldAndReverseTests
{
    [Fact]
    public void PlaceHold_moves_available_to_held_and_is_idempotent()
    {
        var store = new InMemoryLedgerStore();
        var ledger = new LedgerService(store);

        var req = new PlaceHoldRequest(
            HoldKey: "hold-1",
            AccountId: "cust-alice-available",
            ContraAccountId: "cust-alice-held",
            Amount: 40m,
            Currency: "BRL",
            PaymentId: "pay-h1",
            ExpiresInSeconds: 60);

        var first = ledger.PlaceHold(req, "c1");
        var second = ledger.PlaceHold(req, "c2");

        Assert.Equal(first.HoldId, second.HoldId);
        Assert.Equal(HoldStatus.Active, first.Status);
        Assert.Single(store.ListJournals(), j => j.Kind == JournalKind.HoldPlace);
    }

    [Fact]
    public void ReleaseHold_returns_funds_to_available()
    {
        var store = new InMemoryLedgerStore();
        var ledger = new LedgerService(store);

        var hold = ledger.PlaceHold(new PlaceHoldRequest(
            "hold-rel",
            "cust-alice-available",
            "cust-alice-held",
            15m,
            "BRL",
            "pay-r",
            60), "c1");

        var released = ledger.ReleaseHold(hold.HoldId, "c2");
        Assert.Equal(HoldStatus.Released, released.Status);

        var available = ledger.GetBalance("cust-alice-available");
        var held = ledger.GetBalance("cust-alice-held");
        Assert.Equal(0m, available.AvailableSigned);
        Assert.Equal(0m, held.AvailableSigned);
    }

    [Fact]
    public void Reverse_partial_37_percent_stays_balanced()
    {
        var store = new InMemoryLedgerStore();
        var ledger = new LedgerService(store);

        var original = ledger.PostJournal(new PostJournalRequest(
            "orig-1",
            "BRL",
            "pay-med",
            [
                new PostJournalLegRequest("cust-alice-available", 100m, 0),
                new PostJournalLegRequest("cust-bob-available", 0, 100m)
            ]), "c1");

        var reverse = ledger.ReverseJournal(original.JournalId, new ReverseJournalRequest(
            ReverseJournalKey: "med-37",
            Percent: 37m,
            PaymentId: "pay-med"), "c2");

        Assert.Equal(JournalKind.Reverse, reverse.Kind);
        Assert.Equal(reverse.Legs.Sum(l => l.Debit), reverse.Legs.Sum(l => l.Credit));
        Assert.Equal(37m, reverse.Legs.Sum(l => l.Debit));
    }
}
