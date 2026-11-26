using SystemDesign.Payments.Ledger.Domain;

namespace SystemDesign.Payments.Ledger.Tests;

public class JournalBalanceTests
{
    [Fact]
    public void CreateBalanced_rejects_unbalanced_legs()
    {
        var ex = Assert.Throws<LedgerDomainException>(() =>
            Journal.CreateBalanced(
                journalKey: "j1",
                correlationId: "c1",
                currency: "BRL",
                kind: JournalKind.Post,
                legs:
                [
                    new JournalLeg("cust-alice-available", 100m, 0),
                    new JournalLeg("cust-bob-available", 0, 90m)
                ]));

        Assert.Contains("Unbalanced", ex.Message);
    }

    [Fact]
    public void CreateBalanced_accepts_equal_debits_and_credits()
    {
        var journal = Journal.CreateBalanced(
            journalKey: "j2",
            correlationId: "c1",
            currency: "BRL",
            kind: JournalKind.Post,
            legs:
            [
                new JournalLeg("cust-alice-available", 100m, 0),
                new JournalLeg("cust-bob-available", 0, 100m)
            ]);

        Assert.Equal(100m, journal.Legs.Sum(l => l.Debit));
        Assert.Equal(100m, journal.Legs.Sum(l => l.Credit));
    }
}
