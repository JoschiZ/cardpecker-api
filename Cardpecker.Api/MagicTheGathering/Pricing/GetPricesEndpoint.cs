using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Cardpecker.Api.MagicTheGathering.Pricing;

public class GetPricesEndpoint : Endpoint<GetPricesRequest, Results<Ok<CardPricesResponse>, NotFound>>
{
    private readonly PeckerContext _context;

    public GetPricesEndpoint(PeckerContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get(ApiRoutes.MagicTheGathering.GetPrices);
    }

    public override async Task<Results<Ok<CardPricesResponse>, NotFound>> ExecuteAsync(GetPricesRequest req, CancellationToken ct)
    {
        var prices = await _context
            .MagicCardPricingPoints
            .Where(x => x.ScryfallId == req.ScryfallId)
            .AsNoTracking()
            .GroupBy(x => x.PricingProvider)
            .ToDictionaryAsync(x => x.Key, grouping => grouping.Select(pricingPoint => new PricePoint
            {
                CardVersion = pricingPoint.PrintingVersion,
                Price = pricingPoint.Price,
                Date = pricingPoint.PriceDate,
                IsMagicOnline = pricingPoint.IsMagicOnline,
                Currency = pricingPoint.Currency,
            }), cancellationToken: ct);
        return TypedResults.Ok(new CardPricesResponse() { ScryfallId = req.ScryfallId, Prices = prices });
    }
}

public class GetPricesRequest
{
    public Guid ScryfallId { get; set; }
}

public class CardPricesResponse
{
    public required Guid ScryfallId { get; set; }
    public Dictionary<string, IEnumerable<PricePoint>> Prices { get; set; } = [];
}

public class PricePoint
{
    public required string CardVersion { get; set; }
    public required decimal Price { get; set; }
    public required DateOnly Date { get; set; }
    public required bool IsMagicOnline { get; set; }
    public required string Currency { get; set; }
}