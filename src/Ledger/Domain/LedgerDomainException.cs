namespace SystemDesign.Payments.Ledger.Domain;

public sealed class LedgerDomainException : Exception
{
    public LedgerDomainException(string message) : base(message)
    {
    }
}
