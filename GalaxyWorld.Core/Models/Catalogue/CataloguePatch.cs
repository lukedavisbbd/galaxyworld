using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models.Catalogue;

public class CataloguePatch
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> CatalogueName { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> CatalogueSlug { get; init; } = default;
}
