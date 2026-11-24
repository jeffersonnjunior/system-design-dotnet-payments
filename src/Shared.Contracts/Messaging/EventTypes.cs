namespace SystemDesign.Payments.Shared.Contracts.Messaging;

/// <summary>Canonical business event type names (v1).</summary>
public static class EventTypes
{
    public const string PaymentAuthorized = "PaymentAuthorized";
    public const string PaymentDeclined = "PaymentDeclined";
    public const string HoldPlaced = "HoldPlaced";
    public const string JournalPosted = "JournalPosted";
    public const string SettleRequested = "SettleRequested";
    public const string SettleSucceeded = "SettleSucceeded";
    public const string SettleUnknown = "SettleUnknown";
    public const string MedOpened = "MedOpened";
    public const string MedReversed = "MedReversed";
    public const string BreakOpened = "BreakOpened";
}
