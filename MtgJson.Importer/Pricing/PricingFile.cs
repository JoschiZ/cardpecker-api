using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

internal class PricingFile
{
    [JsonPropertyName("data")]
    public Dictionary<Guid, PriceFormats> Data { get; set; } = [];
}