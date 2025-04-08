namespace GalaxyWorldCli.Models;

public class Star
{
    public required int StarId { get; init; }
    public required int? Constellation { get; init; }
    public required string? ProperName { get; init; }
    public required decimal RightAscension { get; init; }
    public required decimal Declination { get; init; }
    public required string PosSrc { get; init; }
    public required decimal? Distance { get; init; }
    public required decimal? X0 { get; init; }
    public required decimal? Y0 { get; init; }
    public required decimal? Z0 { get; init; }
    public required string? DistanceSrc { get; init; }
    public required decimal Magnitude { get; init; }
    public required decimal? AbsoluteMagnitude { get; init; }
    public required decimal? ColourIndex { get; init; }
    public required string MagnitudeSrc { get; init; }
    public required decimal? RadialVelocity { get; init; }
    public required string? RadialVelocitySrc { get; init; }
    public required decimal? ProperMotionRightAscension { get; init; }
    public required decimal? ProperMotionDeclination { get; init; }
    public required string? ProperMotionSrc { get; init; }
    public required decimal? VelocityX { get; init; }
    public required decimal? VelocityY { get; init; }
    public required decimal? VelocityZ { get; init; }
    public required string? SpectralType { get; init; }
    public required string? SpectralTypeSrc { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
