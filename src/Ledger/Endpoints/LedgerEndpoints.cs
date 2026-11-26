using SystemDesign.Payments.Ledger.Application;
using SystemDesign.Payments.Ledger.Domain;
using SystemDesign.Payments.Shared.Contracts.Ids;

namespace SystemDesign.Payments.Ledger.Endpoints;

public static class LedgerEndpoints
{
    public static RouteGroupBuilder MapLedgerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1").WithTags("Ledger");

        group.MapGet("/accounts", (ILedgerStore store) =>
            Results.Ok(store.ListAccounts()));

        group.MapGet("/accounts/{accountId}/balance", (string accountId, LedgerService ledger) =>
        {
            try
            {
                return Results.Ok(ledger.GetBalance(accountId));
            }
            catch (LedgerDomainException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
        });

        group.MapGet("/journals/{journalId}", (string journalId, ILedgerStore store) =>
        {
            var journal = store.GetJournal(journalId);
            return journal is null
                ? Results.NotFound(new { error = "journal not found" })
                : Results.Ok(journal);
        });

        group.MapPost("/journals", (PostJournalRequest body, LedgerService ledger, HttpRequest http) =>
        {
            try
            {
                var correlationId = GetCorrelation(http);
                var journal = ledger.PostJournal(body, correlationId);
                return Results.Ok(journal);
            }
            catch (LedgerDomainException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/journals/{journalId}/reverse", (
            string journalId,
            ReverseJournalRequest body,
            LedgerService ledger,
            HttpRequest http) =>
        {
            try
            {
                var journal = ledger.ReverseJournal(journalId, body, GetCorrelation(http));
                return Results.Ok(journal);
            }
            catch (LedgerDomainException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/holds", (PlaceHoldRequest body, LedgerService ledger, HttpRequest http) =>
        {
            try
            {
                var hold = ledger.PlaceHold(body, GetCorrelation(http));
                return Results.Ok(hold);
            }
            catch (LedgerDomainException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/holds/{holdId}/release", (
            string holdId,
            ReleaseHoldRequest? body,
            LedgerService ledger,
            HttpRequest http) =>
        {
            try
            {
                var hold = ledger.ReleaseHold(holdId, GetCorrelation(http));
                return Results.Ok(hold);
            }
            catch (LedgerDomainException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapGet("/holds/{holdId}", (string holdId, ILedgerStore store) =>
        {
            var hold = store.GetHold(holdId);
            return hold is null
                ? Results.NotFound(new { error = "hold not found" })
                : Results.Ok(hold);
        });

        return group;
    }

    private static string GetCorrelation(HttpRequest http) =>
        http.Headers.TryGetValue(Correlation.HeaderName, out var values)
        && !string.IsNullOrWhiteSpace(values.FirstOrDefault())
            ? values.ToString()
            : Correlation.NewId();
}
