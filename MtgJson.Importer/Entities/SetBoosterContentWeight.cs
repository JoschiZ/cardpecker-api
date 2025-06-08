using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class SetBoosterContentWeight
{
    public int? BoosterIndex { get; set; }

    public string? BoosterName { get; set; }

    public int? BoosterWeight { get; set; }

    public string? SetCode { get; set; }
}
