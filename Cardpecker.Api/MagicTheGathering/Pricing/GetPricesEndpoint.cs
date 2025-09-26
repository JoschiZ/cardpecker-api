using FastEndpoints;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Cardpecker.Api.MagicTheGathering.Pricing;

/// <summary>
/// Gets all known prices of a card by it's scryfall ID
/// </summary>
public class GetPricesEndpoint : Endpoint<GetPricesRequest, Results<Ok<CardPricesResponse>, NotFound>>
{
    private readonly PeckerContext _context;

    /// <inheritdoc />
    public GetPricesEndpoint(PeckerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        AllowAnonymous();
        Get(ApiRoutes.MagicTheGathering.GetPrices);
    }

    /// <inheritdoc />
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
            }), cancellationToken: ct).ConfigureAwait(false);

        if (prices.Count == 0)
        {
            TypedResults.NotFound();
        }
        
        return TypedResults.Ok(new CardPricesResponse() { ScryfallId = req.ScryfallId, Prices = prices });
    }
}

/// <summary>
/// Request to get the price of a specific card
/// </summary>
[UsedImplicitly]
public class GetPricesRequest
{
    /// <summary>
    /// The Scryfall GUID for the searched card
    /// </summary>
    public Guid ScryfallId { get; set; }
}

/// <summary>
/// All known prices of a specific card
/// </summary>
public class CardPricesResponse
{
    /// <summary>
    /// The Scryfall Guid for this card
    /// </summary>
    public required Guid ScryfallId { get; set; }
    /// <summary>
    /// All prices using the marketplace as a key
    /// </summary>
    public Dictionary<string, IEnumerable<PricePoint>> Prices { get; set; } = [];
}

/// <summary>
/// A specific price point of a card for a given vendor
/// </summary>
public class PricePoint
{
    /// <summary>
    /// What kind of printing this card is (foil, etched, normal)
    /// </summary>
    public required string CardVersion { get; set; }
    /// <summary>
    /// The price of the card
    /// </summary>
    public required decimal Price { get; set; }
    /// <summary>
    /// The date of when this price was last updated
    /// </summary>
    public required DateOnly Date { get; set; }
    /// <summary>
    /// If this is a price for magic online
    /// </summary>
    public required bool IsMagicOnline { get; set; }
    /// <summary>
    /// The curryncy
    /// </summary>
    public required string Currency { get; set; }
}