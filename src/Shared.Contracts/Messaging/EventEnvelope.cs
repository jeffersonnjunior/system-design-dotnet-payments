namespace SystemDesign.Payments.Shared.Contracts.Messaging;

/// <summary>
/// Versioned envelope for cross-service business events.
/// Keep this thin — no domain logic here (avoids Shared.Contracts becoming a god assembly).
/// </summary>
public sealed record EventEnvelope<T>(
    string EventType,
    string EventVersion,
    string CorrelationId,
    string? PaymentId,
    DateTimeOffset OccurredAt,
    T Payload)
{
    public static EventEnvelope<T> CreateV1(
        string eventType,
        string correlationId,
        T payload,
        string? paymentId = null) =>
        new(
            EventType: eventType,
            EventVersion: "v1",
            CorrelationId: correlationId,
            PaymentId: paymentId,
            OccurredAt: DateTimeOffset.UtcNow,
            Payload: payload);
}
