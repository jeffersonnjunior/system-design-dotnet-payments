using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SystemDesign.Payments.Shared.Contracts.Ids;

namespace SystemDesign.Payments.Ledger.Tests;

public class LedgerApiIdempotencyTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LedgerApiIdempotencyTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Http_PostJournal_twice_with_same_key_yields_one_journal_id()
    {
        var payload = new
        {
            journalKey = "api-idem-1",
            currency = "BRL",
            paymentId = "pay-api-1",
            legs = new[]
            {
                new { accountId = "cust-alice-available", debit = 10m, credit = 0m },
                new { accountId = "cust-bob-available", debit = 0m, credit = 10m }
            }
        };

        using var req1 = new HttpRequestMessage(HttpMethod.Post, "/v1/journals")
        {
            Content = JsonContent.Create(payload)
        };
        req1.Headers.Add(Correlation.HeaderName, "corr-api-1");

        using var req2 = new HttpRequestMessage(HttpMethod.Post, "/v1/journals")
        {
            Content = JsonContent.Create(payload)
        };
        req2.Headers.Add(Correlation.HeaderName, "corr-api-2");

        var res1 = await _client.SendAsync(req1);
        var res2 = await _client.SendAsync(req2);

        res1.EnsureSuccessStatusCode();
        res2.EnsureSuccessStatusCode();

        await using var s1 = await res1.Content.ReadAsStreamAsync();
        await using var s2 = await res2.Content.ReadAsStreamAsync();
        using var j1 = await JsonDocument.ParseAsync(s1);
        using var j2 = await JsonDocument.ParseAsync(s2);

        var id1 = j1.RootElement.GetProperty("journalId").GetString();
        var id2 = j2.RootElement.GetProperty("journalId").GetString();
        Assert.Equal(id1, id2);
    }
}
