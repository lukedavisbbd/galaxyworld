namespace GalaxyWorld.Core.Models.Star;

public class Star
{
    public required int StarId { get; init; }
    public required int? Constellation { get; init; }
    public required string? ProperName { get; init; }
    public required double RightAscension { get; init; }
    public required double Declination { get; init; }
    public required string PosSrc { get; init; }
    public required double? Distance { get; init; }
    public required double? X0 { get; init; }
    public required double? Y0 { get; init; }
    public required double? Z0 { get; init; }
    public required string? DistanceSrc { get; init; }
    public required double Magnitude { get; init; }
    public required double? AbsoluteMagnitude { get; init; }
    public required double? ColourIndex { get; init; }
    public required string MagnitudeSrc { get; init; }
    public required double? RadialVelocity { get; init; }
    public required string? RadialVelocitySrc { get; init; }
    public required double? ProperMotionRightAscension { get; init; }
    public required double? ProperMotionDeclination { get; init; }
    public required string? ProperMotionSrc { get; init; }
    public required double? VelocityX { get; init; }
    public required double? VelocityY { get; init; }
    public required double? VelocityZ { get; init; }
    public required string? SpectralType { get; init; }
    public required string? SpectralTypeSrc { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
