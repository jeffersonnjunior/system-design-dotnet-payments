namespace SystemDesign.Payments.Ledger.Domain;

/// <summary>Monetary amount. Currency must match across legs of a journal.</summary>
public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Brl(decimal amount) => new(amount, "BRL");

    public Money EnsurePositive() =>
        Amount > 0
            ? this
            : throw new LedgerDomainException("Amount must be greater than zero.");
}
