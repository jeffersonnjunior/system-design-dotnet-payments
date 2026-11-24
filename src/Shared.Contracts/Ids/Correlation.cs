namespace SystemDesign.Payments.Shared.Contracts.Ids;

/// <summary>
/// Correlation helpers. Invariant 15: correlation_id / payment_id cross all services.
/// </summary>
public static class Correlation
{
    public const string HeaderName = "X-Correlation-Id";
    public const string IdempotencyHeaderName = "Idempotency-Key";
    public const string PaymentIdHeaderName = "X-Payment-Id";

    public static string NewId() => Guid.NewGuid().ToString("N");
}
