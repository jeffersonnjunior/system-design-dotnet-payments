namespace SystemDesign.Payments.Ledger.Domain;

/// <summary>Derived position for an account (SoT is journals, not this DTO).</summary>
public sealed record AccountBalance(
    string AccountId,
    string Currency,
    decimal PostedDebits,
    decimal PostedCredits,
    decimal AvailableSigned);
