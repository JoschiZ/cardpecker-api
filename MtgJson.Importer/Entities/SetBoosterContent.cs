using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class SetBoosterContent
{
    public int? BoosterIndex { get; set; }

    public string? BoosterName { get; set; }

    public string? SetCode { get; set; }

    public string? SheetName { get; set; }

    public int? SheetPicks { get; set; }
}
