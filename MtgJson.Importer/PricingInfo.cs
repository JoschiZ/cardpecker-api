using MtgJson.Importer.Pricing;

namespace MtgJson.Importer;

public record PricingInfo(Guid ScryfallId, string Marketplace, string Currency, decimal Price, bool IsMtgOnline, string CardVersion, DateOnly Date);