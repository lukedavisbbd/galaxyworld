using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models.Catalogue;

public class CataloguePatch
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> CatName { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> CatSlug { get; init; } = default;
}
