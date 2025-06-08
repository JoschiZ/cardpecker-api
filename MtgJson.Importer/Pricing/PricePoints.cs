using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

internal class PricePoints
{
    [JsonPropertyName("etched")]
    public Dictionary<DateOnly, decimal>? Etched { get; set; }
    [JsonPropertyName("foil")]
    public Dictionary<DateOnly, decimal>? Foil { get; set; }
    [JsonPropertyName("normal")]
    public Dictionary<DateOnly, decimal>? Normal { get; set; }
}