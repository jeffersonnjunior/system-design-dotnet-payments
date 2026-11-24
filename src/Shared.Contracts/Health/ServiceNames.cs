namespace SystemDesign.Payments.Shared.Contracts.Health;

/// <summary>Stable service names for catalog, compose, and health responses.</summary>
public static class ServiceNames
{
    public const string Gateway = "gateway";
    public const string Accounts = "accounts";
    public const string Risk = "risk";
    public const string Payments = "payments";
    public const string Ledger = "ledger";
    public const string Settlement = "settlement";
    public const string Recon = "recon";
    public const string Extrato = "extrato";
    public const string OpsBreakDesk = "ops-breakdesk";
}
