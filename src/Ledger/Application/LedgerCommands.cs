namespace SystemDesign.Payments.Ledger.Application;

public sealed record PostJournalRequest(
    string JournalKey,
    string Currency,
    string? PaymentId,
    IReadOnlyList<PostJournalLegRequest> Legs);

public sealed record PostJournalLegRequest(string AccountId, decimal Debit, decimal Credit);

public sealed record PlaceHoldRequest(
    string HoldKey,
    string AccountId,
    string ContraAccountId,
    decimal Amount,
    string Currency,
    string? PaymentId,
    int ExpiresInSeconds);

public sealed record ReleaseHoldRequest(string? Reason);

public sealed record ReverseJournalRequest(
    string ReverseJournalKey,
    decimal Percent,
    string? PaymentId);
