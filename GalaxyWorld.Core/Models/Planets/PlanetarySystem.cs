namespace GalaxyWorld.Core.Models.Planets;

public class PlanetarySystem
{
    public required int? NumStars { get; init; }
    public required int? NumPlanets { get; init; }
    public required int? NumMoons { get; init; }
    public required List<Planet> Planets { get; init; }
}
