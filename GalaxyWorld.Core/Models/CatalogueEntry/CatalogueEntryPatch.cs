using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models.CatalogueEntry;

public class CatalogueEntryPatch
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> EntryId { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> EntryDesignation { get; init; } = default;
}
