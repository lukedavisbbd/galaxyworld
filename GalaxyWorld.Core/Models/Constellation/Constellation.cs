namespace GalaxyWorld.Core.Models.Constellation;

public class Constellation
{
    public required int ConstellationId { get; init; }
    public required string ConstellationName { get; init; }
    public required string IauAbbreviation { get; init; }
    public required string NasaAbbreviation { get; init; }
    public required string Genitive { get; init; }
    public required string Origin { get; init; }
    public required string Meaning { get; init; }
}
