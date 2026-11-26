using SystemDesign.Payments.Ledger.Application;
using SystemDesign.Payments.Ledger.Infrastructure;

namespace SystemDesign.Payments.Ledger.Tests;

public class IdempotencyTests
{
    [Fact]
    public void PostJournal_same_key_twice_returns_same_journal_and_single_fact()
    {
        var store = new InMemoryLedgerStore();
        var ledger = new LedgerService(store);
        var request = new PostJournalRequest(
            JournalKey: "idem-1",
            Currency: "BRL",
            PaymentId: "pay-1",
            Legs:
            [
                new PostJournalLegRequest("cust-alice-available", 50m, 0),
                new PostJournalLegRequest("cust-bob-available", 0, 50m)
            ]);

        var first = ledger.PostJournal(request, "corr-1");
        var second = ledger.PostJournal(request, "corr-2");

        Assert.Equal(first.JournalId, second.JournalId);
        Assert.Single(store.ListJournals());
    }

    [Fact]
    public void PostJournal_same_key_does_not_double_balance_effect()
    {
        var store = new InMemoryLedgerStore();
        var ledger = new LedgerService(store);
        var request = new PostJournalRequest(
            JournalKey: "idem-balance",
            Currency: "BRL",
            PaymentId: "pay-2",
            Legs:
            [
                new PostJournalLegRequest("cust-alice-available", 25m, 0),
                new PostJournalLegRequest("cust-bob-available", 0, 25m)
            ]);

        ledger.PostJournal(request, "c1");
        ledger.PostJournal(request, "c1");

        var bob = ledger.GetBalance("cust-bob-available");
        Assert.Equal(25m, bob.PostedCredits);
        Assert.Equal(0m, bob.PostedDebits);
        Assert.Equal(25m, bob.AvailableSigned);
    }
}
