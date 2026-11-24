# Sagas cross-serviço

Cada saga precisa de diagrama de sequência **e** falha no meio de cada hop (código + docs).

| # | Saga | Status |
|---|---|---|
| 1 | PIX authorize → settle → (MED?) | TBD |
| 2 | Cartão auth → capture → settle (capture fail) | TBD |
| 3 | Agendado: schedule → fire → authorize… | TBD |
| 4 | MED parcial pós-settle | TBD |
| 5 | UNKNOWN settle → recon → break/resolve | TBD |

Arquivos por saga entram em `docs/sagas/<nome>.md` quando a FSM for implementada.
