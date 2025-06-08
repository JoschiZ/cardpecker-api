using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

internal class PriceList
{
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }
    [JsonPropertyName("buylist")]
    public PricePoints? BuyList {get; set; }
    [JsonPropertyName("retail")]
    public PricePoints? Retail { get; set; }
}