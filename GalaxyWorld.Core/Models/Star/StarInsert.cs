using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Core.Models.Star;

public class StarInsert
{
    public int? Constellation { get; init; }
    public string? ProperName { get; init; }
    public required double RightAscension { get; init; }
    public required double Declination { get; init; }
    public required string PosSrc { get; init; }
    public double? Distance { get; init; }
    public double? X0 { get; init; }
    public double? Y0 { get; init; }
    public double? Z0 { get; init; }
    public string? DistanceSrc { get; init; }
    public required double Magnitude { get; init; }
    public double? AbsoluteMagnitude { get; init; }
    public double? ColourIndex { get; init; }
    public required string MagnitudeSrc { get; init; }
    public double? RadialVelocity { get; init; }
    public string? RadialVelocitySrc { get; init; }
    public double? ProperMotionRightAscension { get; init; }
    public double? ProperMotionDeclination { get; init; }
    public string? ProperMotionSrc { get; init; }
    public double? VelocityX { get; init; }
    public double? VelocityY { get; init; }
    public double? VelocityZ { get; init; }
    public string? SpectralType { get; init; }
    public string? SpectralTypeSrc { get; init; }
    public IEnumerable<CatalogueEntryInsertWithStar>? CatalogueEntries { get; init; }
}
