# ADR-00 — Conflitos de stakeholder

Status: **draft**  
Decisão: registrar atende / parcial / **recusa** + risco + serviço que absorve.

| Stakeholder | Pedido | Decisão | Serviço que absorve | Risco |
|---|---|---|---|---|
| Produto | p99 PIX ≤ 150–250 ms | Parcial — cortar hops sync; Settlement async | Payments (orquestração) + Gateway | p99 estoura se Risk sincronizar no pico |
| Risco | 100% score fresco | Parcial — budget de timeout; fail-closed default | Risk / Payments | receita vs fraude |
| Contábil | TPV = parceiro D+0 | Recusa de “TPV fantasma” — UNKNOWN não conta como settled | Settlement / Recon / Extrato | CFO board diverge até resolve |
| Growth | merchant ouro fail-open | Parcial — só com teto ADR e métrica | Payments (política) | invariantes 1–6 |
| Ops | auto-heal tudo | Recusa — MANUAL_BREAK com owner | Ops.BreakDesk | fila explode em incidente |
| FinOps | 1 DB compartilhado | Recusa de join cross-serviço — DBs separados no mesmo cluster OK com ADR | infra / cada serviço | custo vs blast radius |
| Legal | retenção + imutabilidade vs esquecimento | Parcial — fatos financeiros imutáveis; PII tokenizada | Accounts / Ledger | GDPR vs audit |

Atualizar cada linha quando a política for codificada.
