namespace SystemDesign.Payments.Ledger.Domain;

public enum AccountKind
{
    CustomerAvailable = 1,
    CustomerHeld = 2,
    SettlementClearing = 3,
    PartnerReceivable = 4,
    FeeRevenue = 5,
    Suspense = 6
}
