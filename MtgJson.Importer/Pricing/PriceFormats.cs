using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

internal class PriceFormats
{
    [JsonPropertyName("mtgo")]
    public Dictionary<string, PriceList> MagicOnline { get; set; } = [];
    
    [JsonPropertyName("paper")]
    public Dictionary<string, PriceList> PaperMagic { get; set; } = [];
}