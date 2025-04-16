namespace GalaxyWorld.Core.Models.Planets;

public class Planet
{
    public required string PlanetName { get; init; }
    public required string SolutionType { get; init; }
    public required bool Controversial { get; init; }
    public required string DiscoveryMethod { get; init; }
    public required int DiscYear { get; init; }
    public required string DiscFacility { get; init; }
    public required string DiscTelescope { get; init; }
    public required decimal? RadiusEarth { get; init; }
    public required decimal? RadiusJupiter { get; init; }
    public required decimal? MassEarth { get; init; }
    public required decimal? MassJupiter { get; init; }
}
