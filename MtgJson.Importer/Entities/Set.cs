using System;
using System.Collections.Generic;

namespace MtgJson.Importer.Entities;

internal partial class Set
{
    public int? BaseSetSize { get; set; }

    public string? Block { get; set; }

    public int? CardsphereSetId { get; set; }

    public string Code { get; set; } = null!;

    public bool? IsFoilOnly { get; set; }

    public bool? IsForeignOnly { get; set; }

    public bool? IsNonFoilOnly { get; set; }

    public bool? IsOnlineOnly { get; set; }

    public bool? IsPartialPreview { get; set; }

    public string? KeyruneCode { get; set; }

    public string? Languages { get; set; }

    public int? McmId { get; set; }

    public int? McmIdExtras { get; set; }

    public string? McmName { get; set; }

    public string? MtgoCode { get; set; }

    public string? Name { get; set; }

    public string? ParentCode { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? TcgplayerGroupId { get; set; }

    public string? TokenSetCode { get; set; }

    public int? TotalSetSize { get; set; }

    public string? Type { get; set; }
}
