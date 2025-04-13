namespace GalaxyWorld.Core.Models.Constellation;

public class ConstellationPatch
{
    public Optional<string> ConName { get; init; } = default;
    public Optional<string> IauAbbr { get; init; } = default;
    public Optional<string> NasaAbbr { get; init; } = default;
    public Optional<string> Genitive { get; init; } = default;
    public Optional<string> Origin { get; init; } = default;
    public Optional<string> Meaning { get; init; } = default;
}
