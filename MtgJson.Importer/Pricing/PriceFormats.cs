using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

internal class PriceFormats
{
    [JsonPropertyName("mtgo")]
    public Dictionary<PricingProvider, PriceList> MagicOnline { get; set; } = [];
    
    [JsonPropertyName("paper")]
    public Dictionary<PricingProvider, PriceList> PaperMagic { get; set; } = [];
}