using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Core.Models.Star;

public class StarInsert
{
    public int? Constellation { get; init; }
    public string? ProperName { get; init; }
    public required decimal RightAscension { get; init; }
    public required decimal Declination { get; init; }
    public required string PosSrc { get; init; }
    public decimal? Distance { get; init; }
    public decimal? X0 { get; init; }
    public decimal? Y0 { get; init; }
    public decimal? Z0 { get; init; }
    public string? DistanceSrc { get; init; }
    public required decimal Magnitude { get; init; }
    public decimal? AbsoluteMagnitude { get; init; }
    public decimal? ColourIndex { get; init; }
    public required string MagnitudeSrc { get; init; }
    public decimal? RadialVelocity { get; init; }
    public string? RadialVelocitySrc { get; init; }
    public decimal? ProperMotionRightAscension { get; init; }
    public decimal? ProperMotionDeclination { get; init; }
    public string? ProperMotionSrc { get; init; }
    public decimal? VelocityX { get; init; }
    public decimal? VelocityY { get; init; }
    public decimal? VelocityZ { get; init; }
    public string? SpectralType { get; init; }
    public string? SpectralTypeSrc { get; init; }
    public IEnumerable<CatalogueEntryInsertWithStar>? CatalogueEntries { get; init; }
}
