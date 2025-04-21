namespace GalaxyWorld.Core.Models.CatalogueEntry;

public class CatalogueEntry
{
    public required int StarId { get; init; }
    public required int CatalogueId { get; init; }
    public string? EntryId { get; init; }
    public string? EntryDesignation { get; init; }
}
