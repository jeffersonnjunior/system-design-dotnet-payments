using SystemDesign.Payments.Shared.Contracts.Health;
using SystemDesign.Payments.Shared.Contracts.Ids;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey(Correlation.HeaderName))
        context.Request.Headers[Correlation.HeaderName] = Correlation.NewId();

    context.Response.Headers[Correlation.HeaderName] =
        context.Request.Headers[Correlation.HeaderName].ToString();

    await next();
});

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    service = ServiceNames.Ledger,
    role = "SoT for balance/position; double-entry journals, holds, reverse",
    status = "up",
    commands = new[] { "PostJournal", "PlaceHold", "ReleaseHold", "Reverse" }
}));

app.Run();

public partial class Program;
