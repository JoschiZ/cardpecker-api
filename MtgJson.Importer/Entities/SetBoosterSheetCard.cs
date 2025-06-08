using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class SetBoosterSheetCard
{
    public string? BoosterName { get; set; }

    public string CardUuid { get; set; } = null!;

    public long? CardWeight { get; set; }

    public string? SetCode { get; set; }

    public string? SheetName { get; set; }
}
