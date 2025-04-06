namespace GalaxyWorld.Models.Dbos;

public class Star
{
    public required int StarId { get; init; }
    public required int? AthygId { get; set; }
    public required string? Tycho2Id { get; set; }
    public required long? GaiaDr3Id { get; set; }
    public required int? HygV3Id { get; set; }
    public required int? HipparcosId { get; set; }
    public required int? HenryDraperId { get; set; }
    public required int? HarvardYaleId { get; set; }
    public required string? GlieseId { get; set; }
    public required string? BayerId { get; set; }
    public required int? FlamsteedId { get; set; }
    public required string? Constellation { get; set; }
    public required string? ProperName { get; set; }
    public required decimal RightAscension { get; set; }
    public required decimal Declination { get; set; }
    public required string PosSrc { get; set; }
    public required decimal? Distance { get; set; }
    public required decimal? X0 { get; set; }
    public required decimal? Y0 { get; set; }
    public required decimal? Z0 { get; set; }
    public required string? DistanceSrc { get; set; }
    public required decimal Magnitude { get; set; }
    public required decimal? AbsoluteMagnitude { get; set; }
    public required decimal? ColourIndex { get; set; }
    public required string MagnitudeSrc { get; set; }
    public required decimal? RadialVelocity { get; set; }
    public required string? RadialVelocitySrc { get; set; }
    public required decimal? ProperMotionRightAscension { get; set; }
    public required decimal? ProperMotionDeclination { get; set; }
    public required string? ProperMotionSrc { get; set; }
    public required decimal? VelocityX { get; set; }
    public required decimal? VelocityY { get; set; }
    public required decimal? VelocityZ { get; set; }
    public required string? SpectralType { get; set; }
    public required string? SpectralTypeSrc { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
