using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models.Star;

public class StarPatch
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<int?> ConstellationId { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> ProperName { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<decimal> RightAscension { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<decimal> Declination { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<decimal?> Distance { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<Vector?> PositionCartesian { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<decimal> Magnitude { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<Vector?> VelocityCircular { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<Vector?> VelocityCartesian { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<decimal?> ColourIndex { get; init; } = default;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> SpectralType { get; init; } = default;
}
