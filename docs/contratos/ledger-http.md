# Ledger HTTP (v1)

Base: `http://localhost:5104`

| Method | Path | Idempotency |
|---|---|---|
| GET | `/v1/accounts` | — |
| GET | `/v1/accounts/{accountId}/balance` | — |
| POST | `/v1/journals` | `journalKey` |
| GET | `/v1/journals/{journalId}` | — |
| POST | `/v1/journals/{journalId}/reverse` | `reverseJournalKey` |
| POST | `/v1/holds` | `holdKey` |
| POST | `/v1/holds/{holdId}/release` | `hold-release:{holdKey}` |
| GET | `/v1/holds/{holdId}` | — |

Header obrigatório recomendado: `X-Correlation-Id`.

Seed accounts: `cust-alice-available`, `cust-alice-held`, `cust-bob-available`, `settlement-clearing`, `partner-receivable`, `fee-revenue`.
