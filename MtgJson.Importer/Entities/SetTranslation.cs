using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class SetTranslation
{
    public string? Language { get; set; }

    public string? SetCode { get; set; }

    public string? Translation { get; set; }
}
