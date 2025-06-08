using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class CardRuling
{
    public DateOnly? Date { get; set; }

    public string? Text { get; set; }

    public string Uuid { get; set; } = null!;
}
