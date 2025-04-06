namespace GalaxyWorld.Models.Dbos;

public class StarInsert
{
    public int? AthygId { get; set; }
    public string? Tycho2Id { get; set; }
    public long? GaiaDr3Id { get; set; }
    public int? HygV3Id { get; set; }
    public int? HipparcosId { get; set; }
    public int? HenryDraperId { get; set; }
    public int? HarvardYaleId { get; set; }
    public string? GlieseId { get; set; }
    public string? BayerId { get; set; }
    public int? FlamsteedId { get; set; }
    public string? Constellation { get; set; }
    public string? ProperName { get; set; }
    public required decimal RightAscension { get; set; }
    public required decimal Declination { get; set; }
    public required string PosSrc { get; set; }
    public decimal? Distance { get; set; }
    public decimal? X0 { get; set; }
    public decimal? Y0 { get; set; }
    public decimal? Z0 { get; set; }
    public string? DistanceSrc { get; set; }
    public required decimal Magnitude { get; set; }
    public decimal? AbsoluteMagnitude { get; set; }
    public decimal? ColourIndex { get; set; }
    public required string MagnitudeSrc { get; set; }
    public decimal? RadialVelocity { get; set; }
    public string? RadialVelocitySrc { get; set; }
    public decimal? ProperMotionRightAscension { get; set; }
    public decimal? ProperMotionDeclination { get; set; }
    public string? ProperMotionSrc { get; set; }
    public decimal? VelocityX { get; set; }
    public decimal? VelocityY { get; set; }
    public decimal? VelocityZ { get; set; }
    public string? SpectralType { get; set; }
    public string? SpectralTypeSrc { get; set; }
}
