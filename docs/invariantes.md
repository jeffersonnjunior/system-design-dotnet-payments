# Invariantes de negócio (1–16)

Cada invariante precisa de **serviço dono + mecanismo**. Atualizar esta tabela conforme o código fechar.

| # | Invariante | Dono | Mecanismo (status) |
|---|---|---|---|
| 1 | Só Ledger é SoT de saldo/posição | Ledger | TBD — journals/accounts chart |
| 2 | Mudança de posição = double-entry balanceada | Ledger | TBD — reject unbalanced legs |
| 3 | Intenção idempotente (`Idempotency-Key` + hash) | Gateway + Payments + Ledger | Header em Shared.Contracts; store TBD |
| 4 | Sem dual-write: efeito + outbox no mesmo commit | cada serviço dono | TBD — outbox table |
| 5 | Estorno/MED parcial: legs reversas proporcionais | Ledger + Payments | TBD — Reverse(% ) |
| 6 | Hold/PENDING com expiração | Ledger | TBD — hold expiry job |
| 7 | Accounts dono de limite/bloqueio/status | Accounts | TBD — GetAccountDecision |
| 8 | Risk recomenda; Payments decide commit | Risk / Payments | Skeleton roles documented in `/` |
| 9 | Settlement não inventa lançamento | Settlement | TBD — only on commanded settle |
| 10 | Recon abre break; não corrige saldo calado | Recon / Ops | TBD — OpenBreak |
| 11 | Extrato atrasa; nunca autoriza | Extrato / Payments | Skeleton role on Extrato `/` |
| 12 | AUTH ≠ CAPTURE; capture fail tem regra de hold | Payments + Ledger | TBD — card FSM |
| 13 | Agendado: cancel antes ≠ settle; após início FSM | Payments (+ Scheduler?) | TBD |
| 14 | Parceiro UNKNOWN → estado + recon; sem 2º settle | Settlement / Recon | TBD |
| 15 | `correlation_id` / `payment_id` atravessa tudo | todos | Middleware stub + Correlation helpers |
| 16 | PII/chave PIX fora de log; fatos replayáveis | todos | TBD — redaction + journal ids |

Fonte: card Capstone Principal — plataforma de pagamentos.
