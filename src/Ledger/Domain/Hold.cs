namespace SystemDesign.Payments.Ledger.Domain;

public enum HoldStatus
{
    Active = 1,
    Released = 2,
    Captured = 3,
    Expired = 4
}

/// <summary>Pending position (invariant 6). Expiry releases without disappearing money.</summary>
public sealed class Hold
{
    public required string HoldId { get; init; }
    public required string HoldKey { get; init; }
    public required string AccountId { get; init; }
    public required string ContraAccountId { get; init; }
    public required Money Amount { get; init; }
    public required string? PaymentId { get; init; }
    public required string CorrelationId { get; init; }
    public required DateTimeOffset PlacedAt { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required HoldStatus Status { get; set; }
    public string? PlaceJournalId { get; set; }
    public string? ReleaseJournalId { get; set; }
}
