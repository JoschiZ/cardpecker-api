using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class CardForeignDatum
{
    public string? FaceName { get; set; }

    public string? FlavorText { get; set; }

    public string? Identifiers { get; set; }

    public string? Language { get; set; }

    public int? MultiverseId { get; set; }

    public string? Name { get; set; }

    public string? Text { get; set; }

    public string? Type { get; set; }

    public string? Uuid { get; set; }
}
