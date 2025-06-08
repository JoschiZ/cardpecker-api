using System.Text.Json.Serialization;

namespace MtgJson.Importer.Pricing;

public enum PricingProvider
{
    [JsonStringEnumMemberName("cardhoarder")]
    CardHoarder,
    [JsonStringEnumMemberName("cardkingdom")]
    CardKingdom,
    [JsonStringEnumMemberName("cardmarket")]
    CardMarket,
    [JsonStringEnumMemberName("cardsphere")]
    CardSphere,
    [JsonStringEnumMemberName("tcgplayer")]
    TcgPlayer
}