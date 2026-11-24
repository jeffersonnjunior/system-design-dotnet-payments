# System Design — Plataforma de Pagamentos (.NET)

Monorepo multi-serviço (Capstone Principal): Gateway, Accounts, Risk, Payments, Ledger, Settlement, Recon, Extrato, Ops.BreakDesk.

## Abrir no Visual Studio

1. Abra **`SystemDesign.Payments.sln`** (formato clássico `.sln`, não `.slnx`).
2. Projetos ficam sob a solution folder `src`.
3. Multi-startup: use o perfil em **`SystemDesign.Payments.slnLaunch`** (VS 17.8+), ou clique direito na Solution → *Configure Startup Projects…*.
4. Portas locais (launchSettings):

| Serviço | URL |
|---|---|
| Gateway | http://localhost:5100 |
| Accounts | http://localhost:5101 |
| Risk | http://localhost:5102 |
| Payments | http://localhost:5103 |
| Ledger | http://localhost:5104 |
| Settlement | http://localhost:5105 |
| Recon | http://localhost:5106 |
| Extrato | http://localhost:5107 |
| Ops.BreakDesk | http://localhost:5108 |

Health: `GET /health` em cada serviço.

## CLI

```bash
dotnet restore SystemDesign.Payments.sln
dotnet build SystemDesign.Payments.sln
dotnet run --project src/Gateway
```

```bash
make build
make up      # docker compose — 9 processos + Postgres + RabbitMQ
make health
make demo    # stub até o PIX E2E
```

## Estrutura

```text
SystemDesign.Payments.sln
src/
  Shared.Contracts/
  Gateway/ Accounts/ Risk/ Payments/ Ledger/
  Settlement/ Recon/ Extrato/ Ops.BreakDesk/
docs/          # invariantes, ADR, C4, sagas
infra/         # compose + Dockerfile + init SQL (1 DB/serviço)
tests/         # unit / integration / e2e (a preencher)
```

## Invariantes (resumo)

- **Ledger** é o único SoT de saldo (double-entry).
- **Payments** orquestra; **não** guarda saldo.
- **Risk** recomenda; Payments decide.
- Sem dual-write: efeito monetário + outbox no mesmo commit.
- Extrato **não** autoriza pagamento.

Ver `docs/invariantes.md` e `docs/adr/ADR-00-conflitos.md`.
