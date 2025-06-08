using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class SetBoosterSheet
{
    public string? BoosterName { get; set; }

    public string? SetCode { get; set; }

    public bool? SheetHasBalanceColors { get; set; }

    public bool? SheetIsFoil { get; set; }

    public string? SheetName { get; set; }
}
