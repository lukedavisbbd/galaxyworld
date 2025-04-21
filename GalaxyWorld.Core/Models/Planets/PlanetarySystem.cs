namespace GalaxyWorld.Core.Models.Planets;

public class PlanetarySystem
{
    public required int? SystemNumStars { get; init; }
    public required int? SystemNumPlanets { get; init; }
    public required int? SystemNumMoons { get; init; }
    public required List<Planet> Planets { get; init; }
}
