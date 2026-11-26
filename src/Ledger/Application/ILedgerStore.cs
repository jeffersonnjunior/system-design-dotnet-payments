using SystemDesign.Payments.Ledger.Domain;

namespace SystemDesign.Payments.Ledger.Application;

public interface ILedgerStore
{
    LedgerAccount? GetAccount(string accountId);
    void UpsertAccount(LedgerAccount account);
    IReadOnlyList<LedgerAccount> ListAccounts();

    Journal? FindJournalByKey(string journalKey);
    Journal? GetJournal(string journalId);
    void AppendJournal(Journal journal);
    IReadOnlyList<Journal> ListJournals();

    Hold? FindHoldByKey(string holdKey);
    Hold? GetHold(string holdId);
    void UpsertHold(Hold hold);

    AccountBalance GetBalance(string accountId);
}
