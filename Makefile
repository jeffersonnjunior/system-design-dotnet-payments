COMPOSE := docker compose -f infra/docker-compose.yml
SLN := SystemDesign.Payments.sln

.PHONY: restore build up down logs demo health prove-idempotency prove-no-double-settle prove-partial-med prove-unknown-settle prove-saga-card-capture-fail

restore:
	dotnet restore $(SLN)

build:
	dotnet restore $(SLN)
	dotnet build $(SLN) -c Release

up:
	$(COMPOSE) up --build -d

down:
	$(COMPOSE) down -v

logs:
	$(COMPOSE) logs -f --tail=100

health:
	@echo Gateway    && curl -sf http://localhost:5100/health && echo
	@echo Accounts   && curl -sf http://localhost:5101/health && echo
	@echo Risk       && curl -sf http://localhost:5102/health && echo
	@echo Payments   && curl -sf http://localhost:5103/health && echo
	@echo Ledger     && curl -sf http://localhost:5104/health && echo
	@echo Settlement && curl -sf http://localhost:5105/health && echo
	@echo Recon      && curl -sf http://localhost:5106/health && echo
	@echo Extrato    && curl -sf http://localhost:5107/health && echo
	@echo Ops        && curl -sf http://localhost:5108/health && echo

demo:
	@echo "demo stub — PIX E2E lands after Payments/Ledger/Settlement wiring"
	@$(MAKE) health

prove-idempotency:
	@echo "TODO: 2x same Idempotency-Key -> 1 journal"

prove-no-double-settle:
	@echo "TODO: settlement replay must not double-post"

prove-partial-med:
	@echo "TODO: partial MED legs balanced"

prove-unknown-settle:
	@echo "TODO: UNKNOWN stays observable + break/recon, no second settle"

prove-saga-card-capture-fail:
	@echo "TODO: auth ok, capture fail, hold released/compensated"
