namespace SystemDesign.Payments.Ledger.Domain;

/// <summary>Chart-of-accounts entry. Balance is derived from posted legs, never written directly.</summary>
public sealed class LedgerAccount
{
    public required string AccountId { get; init; }
    public required string OwnerRef { get; init; }
    public required AccountKind Kind { get; init; }
    public required string Currency { get; init; }
    public string? DisplayName { get; init; }
}
