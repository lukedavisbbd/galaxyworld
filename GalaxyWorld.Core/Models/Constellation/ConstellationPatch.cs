using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models.Constellation;

public class ConstellationPatch
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> ConName { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> IauAbbr { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> NasaAbbr { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> Genitive { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> Origin { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string> Meaning { get; init; } = default;
}
