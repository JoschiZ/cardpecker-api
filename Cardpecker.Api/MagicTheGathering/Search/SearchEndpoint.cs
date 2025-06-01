using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ScryfallApi.Client;
using ScryfallApi.Client.Models;

namespace Cardpecker.Api.MagicTheGathering.Search;

public class SearchEndpoint : Endpoint<SearchEndpoint.Request, Results<Ok<IEnumerable<Card>>, NotFound>>
{
    private readonly ScryfallApiClient _scryfallApiClient;

    public SearchEndpoint(ScryfallApiClient scryfallApiClient)
    {
        _scryfallApiClient = scryfallApiClient;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get(ApiRoutes.MagicTheGathering.Search);
    }

    public override async Task<Results<Ok<IEnumerable<Card>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {

        var searchResult = await _scryfallApiClient.Cards.Named(req.Query, true);
        
        if (searchResult.IsSuccess)
        {
            return TypedResults.Ok<IEnumerable<Card>>([searchResult.Value]);
        }

        var syntaxSearch = await _scryfallApiClient.Cards.Search(req.Query, 0, new SearchOptions()
        {
            Sort = SearchOptions.CardSort.Name,
            Mode = SearchOptions.RollupMode.Cards,
        });

        if (syntaxSearch.IsSuccess)
        {
            return TypedResults.Ok(syntaxSearch.Value.Data.Take(10));;
        }
        
        return TypedResults.NotFound();
    }

    public class Request
    {
        public required string Query { get; set; }
    }
}