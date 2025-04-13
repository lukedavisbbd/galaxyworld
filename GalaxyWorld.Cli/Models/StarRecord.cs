namespace GalaxyWorld.Cli.Models;

public class StarRecord
{
    public int? AthygId { get; set; }
    public string? Tycho2Id { get; set; }
    public long? GaiaDr3Id { get; set; }
    public int? HygV3Id { get; set; }
    public int? HipparcosId { get; set; }
    public int? HenryDraperId { get; set; }
    public int? HarvardYaleId { get; set; }
    public string? GlieseId { get; set; }
    public string? BayerDesignation { get; set; }
    public int? FlamsteedDesignation { get; set; }
    public string? Constellation { get; set; }
    public string? ProperName { get; set; }
    public required double RightAscension { get; set; }
    public required double Declination { get; set; }
    public required string PosSrc { get; set; }
    public double? Distance { get; set; }
    public double? X0 { get; set; }
    public double? Y0 { get; set; }
    public double? Z0 { get; set; }
    public string? DistanceSrc { get; set; }
    public required double Magnitude { get; set; }
    public double? AbsoluteMagnitude { get; set; }
    public double? ColourIndex { get; set; }
    public required string MagnitudeSrc { get; set; }
    public double? RadialVelocity { get; set; }
    public string? RadialVelocitySrc { get; set; }
    public double? ProperMotionRightAscension { get; set; }
    public double? ProperMotionDeclination { get; set; }
    public string? ProperMotionSrc { get; set; }
    public double? VelocityX { get; set; }
    public double? VelocityY { get; set; }
    public double? VelocityZ { get; set; }
    public string? SpectralType { get; set; }
    public string? SpectralTypeSrc { get; set; }

    public static Dictionary<string, string> HeaderMappings = new Dictionary<string, string>
    {
        { "AthygId", "id" },
        { "Tycho2Id", "tyc" },
        { "GaiaDr3Id", "gaia" },
        { "HygV3Id", "hyg" },
        { "HipparcosId", "hip" },
        { "HenryDraperId", "hd" },
        { "HarvardYaleId", "hr" },
        { "GlieseId", "gl" },
        { "BayerDesignation", "bayer" },
        { "FlamsteedDesignation", "flam" },
        { "Constellation", "con" },
        { "ProperName", "proper" },
        { "RightAscension", "ra" },
        { "Declination", "dec" },
        { "PosSrc", "pos_src" },
        { "Distance", "dist" },
        { "X0", "x0" },
        { "Y0", "y0" },
        { "Z0", "z0" },
        { "DistanceSrc", "dist_src" },
        { "Magnitude", "mag" },
        { "AbsoluteMagnitude", "absmag" },
        { "ColourIndex", "ci" },
        { "MagnitudeSrc", "mag_src" },
        { "RadialVelocity", "rv" },
        { "RadialVelocitySrc", "rv_src" },
        { "ProperMotionRightAscension", "pm_ra" },
        { "ProperMotionDeclination", "pm_dec" },
        { "ProperMotionSrc", "pm_src" },
        { "VelocityX", "vx" },
        { "VelocityY", "vy" },
        { "VelocityZ", "vz" },
        { "SpectralType", "spect" },
        { "SpectralTypeSrc", "spect_src" },
    };
}
