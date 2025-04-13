﻿namespace GalaxyWorld.Core.Models.CatalogueEntry;

public class CatalogueEntryInsertWithStar
{
    public required int CatId { get; init; }
    public string? EntryId { get; init; }
    public string? EntryDesignation { get; init; }
}
