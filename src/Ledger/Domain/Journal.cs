namespace SystemDesign.Payments.Ledger.Domain;

public enum JournalKind
{
    Post = 1,
    HoldPlace = 2,
    HoldRelease = 3,
    Reverse = 4
}

/// <summary>Immutable posted journal. Invariant 2: legs must balance.</summary>
public sealed class Journal
{
    public required string JournalId { get; init; }
    public required string JournalKey { get; init; }
    public required string? PaymentId { get; init; }
    public required string CorrelationId { get; init; }
    public required string Currency { get; init; }
    public required JournalKind Kind { get; init; }
    public required IReadOnlyList<JournalLeg> Legs { get; init; }
    public required DateTimeOffset PostedAt { get; init; }
    public string? ReversesJournalId { get; init; }
    public string? HoldId { get; init; }

    public static Journal CreateBalanced(
        string journalKey,
        string correlationId,
        string currency,
        JournalKind kind,
        IReadOnlyList<JournalLeg> legs,
        string? paymentId = null,
        string? reversesJournalId = null,
        string? holdId = null)
    {
        if (string.IsNullOrWhiteSpace(journalKey))
            throw new LedgerDomainException("JournalKey is required (idempotency).");

        if (legs.Count < 2)
            throw new LedgerDomainException("Journal requires at least two legs.");

        foreach (var leg in legs)
            leg.Validate();

        var debit = legs.Sum(l => l.Debit);
        var credit = legs.Sum(l => l.Credit);
        if (debit != credit)
            throw new LedgerDomainException($"Unbalanced journal: debit={debit} credit={credit}.");

        if (debit <= 0)
            throw new LedgerDomainException("Journal total must be greater than zero.");

        return new Journal
        {
            JournalId = Guid.NewGuid().ToString("N"),
            JournalKey = journalKey,
            PaymentId = paymentId,
            CorrelationId = correlationId,
            Currency = currency,
            Kind = kind,
            Legs = legs.ToArray(),
            PostedAt = DateTimeOffset.UtcNow,
            ReversesJournalId = reversesJournalId,
            HoldId = holdId
        };
    }
}
