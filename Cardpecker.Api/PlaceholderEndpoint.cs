using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ScryfallApi.Client;
using ScryfallApi.Client.Models;

namespace Cardpecker.Api;

public class PlaceholderEndpoint : Endpoint<RequestObj, Ok<Card>>
{
    private readonly ScryfallApiClient _apiClient;

    public PlaceholderEndpoint(ScryfallApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override void Configure()
    {
        Get("hello-world");
        AllowAnonymous();
    }

    public override async Task<Ok<Card>> ExecuteAsync(RequestObj req, CancellationToken ct)
    {
        var cards = await _apiClient.Cards.Search(req.Hallo, 0, SearchOptions.CardSort.Cmc);
        
        return TypedResults.Ok(cards.Data.FirstOrDefault());
    }
}
public record RequestObj(string Hallo);
public record ResponseObj(string Hallo);