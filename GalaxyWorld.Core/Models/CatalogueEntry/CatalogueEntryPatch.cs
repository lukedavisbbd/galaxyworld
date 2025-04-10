namespace GalaxyWorld.Core.Models.CatalogueEntry;

public class CatalogueEntryPatch
{
    public Optional<string?> EntryId { get; init; } = default;
    public Optional<string?> EntryDesignation { get; init; } = default;
}
