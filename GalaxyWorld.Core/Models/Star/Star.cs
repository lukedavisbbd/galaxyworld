namespace GalaxyWorld.Core.Models.Star;

public class Star
{
    public required int StarId { get; init; }
    public required int? ConstellationId { get; init; }
    public required string? ProperName { get; init; }
    public required decimal RightAscension { get; init; }
    public required decimal Declination { get; init; }
    public required decimal? Distance { get; init; }
    public required Vector? PositionCartesian { get; init; }
    public required decimal Magnitude { get; init; }
    public required Vector? VelocityCircular { get; init; }
    public required Vector? VelocityCartesian { get; init; }
    public required decimal? ColourIndex { get; init; }
    public required string? SpectralType { get; init; }
}
