namespace SystemDesign.Payments.Ledger.Domain;

/// <summary>One side of a double-entry journal. Exactly one of Debit/Credit must be &gt; 0.</summary>
public sealed record JournalLeg(string AccountId, decimal Debit, decimal Credit)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountId))
            throw new LedgerDomainException("Leg AccountId is required.");

        if (Debit < 0 || Credit < 0)
            throw new LedgerDomainException("Debit/Credit cannot be negative.");

        if (Debit == 0 && Credit == 0)
            throw new LedgerDomainException("Leg must have debit or credit.");

        if (Debit > 0 && Credit > 0)
            throw new LedgerDomainException("Leg cannot have both debit and credit.");
    }
}
