using System.Collections.Concurrent;
using SystemDesign.Payments.Ledger.Application;
using SystemDesign.Payments.Ledger.Domain;

namespace SystemDesign.Payments.Ledger.Infrastructure;

/// <summary>
/// Process-local SoT for the Ledger slice. Replace with Postgres (DB ledger) without changing command shapes.
/// Idempotency is keyed by JournalKey / HoldKey (invariant 3).
/// </summary>
public sealed class InMemoryLedgerStore : ILedgerStore
{
    private readonly ConcurrentDictionary<string, LedgerAccount> _accounts = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Journal> _journalsById = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _journalIdByKey = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Hold> _holdsById = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _holdIdByKey = new(StringComparer.Ordinal);
    private readonly object _writeLock = new();

    public InMemoryLedgerStore()
    {
        SeedChartOfAccounts();
    }

    public LedgerAccount? GetAccount(string accountId) =>
        _accounts.TryGetValue(accountId, out var account) ? account : null;

    public void UpsertAccount(LedgerAccount account) =>
        _accounts[account.AccountId] = account;

    public IReadOnlyList<LedgerAccount> ListAccounts() =>
        _accounts.Values.OrderBy(a => a.AccountId).ToArray();

    public Journal? FindJournalByKey(string journalKey) =>
        _journalIdByKey.TryGetValue(journalKey, out var id) ? GetJournal(id) : null;

    public Journal? GetJournal(string journalId) =>
        _journalsById.TryGetValue(journalId, out var journal) ? journal : null;

    public void AppendJournal(Journal journal)
    {
        lock (_writeLock)
        {
            if (_journalIdByKey.ContainsKey(journal.JournalKey))
                throw new LedgerDomainException($"JournalKey already exists: {journal.JournalKey}");

            foreach (var leg in journal.Legs)
            {
                if (!_accounts.ContainsKey(leg.AccountId))
                    throw new LedgerDomainException($"Unknown account: {leg.AccountId}");
            }

            _journalsById[journal.JournalId] = journal;
            _journalIdByKey[journal.JournalKey] = journal.JournalId;
        }
    }

    public IReadOnlyList<Journal> ListJournals() =>
        _journalsById.Values.OrderBy(j => j.PostedAt).ToArray();

    public Hold? FindHoldByKey(string holdKey) =>
        _holdIdByKey.TryGetValue(holdKey, out var id) ? GetHold(id) : null;

    public Hold? GetHold(string holdId) =>
        _holdsById.TryGetValue(holdId, out var hold) ? hold : null;

    public void UpsertHold(Hold hold)
    {
        lock (_writeLock)
        {
            _holdsById[hold.HoldId] = hold;
            _holdIdByKey[hold.HoldKey] = hold.HoldId;
        }
    }

    public AccountBalance GetBalance(string accountId)
    {
        if (!_accounts.TryGetValue(accountId, out var account))
            throw new LedgerDomainException($"Unknown account: {accountId}");

        decimal debits = 0;
        decimal credits = 0;
        foreach (var journal in _journalsById.Values)
        {
            foreach (var leg in journal.Legs.Where(l => l.AccountId == accountId))
            {
                debits += leg.Debit;
                credits += leg.Credit;
            }
        }

        // Customer liability-style: credit increases available; held accounts are also credit-normal here.
        var signed = credits - debits;
        return new AccountBalance(accountId, account.Currency, debits, credits, signed);
    }

    private void SeedChartOfAccounts()
    {
        UpsertAccount(new LedgerAccount
        {
            AccountId = "cust-alice-available",
            OwnerRef = "customer:alice",
            Kind = AccountKind.CustomerAvailable,
            Currency = "BRL",
            DisplayName = "Alice available"
        });
        UpsertAccount(new LedgerAccount
        {
            AccountId = "cust-alice-held",
            OwnerRef = "customer:alice",
            Kind = AccountKind.CustomerHeld,
            Currency = "BRL",
            DisplayName = "Alice held"
        });
        UpsertAccount(new LedgerAccount
        {
            AccountId = "cust-bob-available",
            OwnerRef = "customer:bob",
            Kind = AccountKind.CustomerAvailable,
            Currency = "BRL",
            DisplayName = "Bob available"
        });
        UpsertAccount(new LedgerAccount
        {
            AccountId = "settlement-clearing",
            OwnerRef = "platform:settlement",
            Kind = AccountKind.SettlementClearing,
            Currency = "BRL",
            DisplayName = "Settlement clearing"
        });
        UpsertAccount(new LedgerAccount
        {
            AccountId = "partner-receivable",
            OwnerRef = "partner:spi",
            Kind = AccountKind.PartnerReceivable,
            Currency = "BRL",
            DisplayName = "Partner receivable"
        });
        UpsertAccount(new LedgerAccount
        {
            AccountId = "fee-revenue",
            OwnerRef = "platform:fees",
            Kind = AccountKind.FeeRevenue,
            Currency = "BRL",
            DisplayName = "Fee revenue"
        });
    }
}
