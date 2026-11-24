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
    service = ServiceNames.Payments,
    role = "orchestrates payment intention/FSM; owns payment_id; never stores balance",
    status = "up",
    products = new[] { "pix", "card", "scheduled", "internal_transfer", "med" }
}));

app.Run();

public partial class Program;
