namespace GalaxyWorld.Core.Models.Constellation;

public class ConstellationInsert
{
    public required string ConName { get; init; }
    public required string IauAbbr { get; init; }
    public required string NasaAbbr { get; init; }
    public required string Genitive { get; init; }
    public required string Origin { get; init; }
    public required string Meaning { get; init; }
}
