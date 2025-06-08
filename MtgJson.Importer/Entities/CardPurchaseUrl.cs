using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class CardPurchaseUrl
{
    public string? CardKingdom { get; set; }

    public string? CardKingdomEtched { get; set; }

    public string? CardKingdomFoil { get; set; }

    public string? Cardmarket { get; set; }

    public string? Tcgplayer { get; set; }

    public string? TcgplayerEtched { get; set; }

    public string? Uuid { get; set; }
}
