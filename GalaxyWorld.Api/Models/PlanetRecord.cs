using System.Text.Json.Serialization;

namespace GalaxyWorld.Api.Models;

public class PlanetRecord
{
    [JsonPropertyName("pl_name")]
    public required string PlanetName { get; init; }
    [JsonPropertyName("pl_controv_flag")]
    public required int Controverial { get; init; }
    [JsonPropertyName("soltype")]
    public required string SolutionType { get; init; }
    [JsonPropertyName("discoverymethod")]
    public required string DiscoveryMethod { get; init; }
    [JsonPropertyName("disc_year")]
    public required int DiscoveryYear { get; init; }
    [JsonPropertyName("disc_facility")]
    public required string DiscoveryFacility { get; init; }
    [JsonPropertyName("disc_telescope")]
    public required string DiscoveryTelescope { get; init; }
    [JsonPropertyName("pl_rade")]
    public required decimal? RadiusEarth { get; init; }
    [JsonPropertyName("pl_radj")]
    public required decimal? RadiusJupiter { get; init; }
    [JsonPropertyName("pl_masse")]
    public required decimal? MassEarth { get; init; }
    [JsonPropertyName("pl_massj")]
    public required decimal? MassJupiter { get; init; }
    [JsonPropertyName("sy_snum")]
    public required int? SystemNumStars { get; init; }
    [JsonPropertyName("sy_pnum")]
    public required int? SystemNumPlanets { get; init; }
    [JsonPropertyName("sy_mnum")]
    public required int? SystemNumMoons { get; init; }
}
