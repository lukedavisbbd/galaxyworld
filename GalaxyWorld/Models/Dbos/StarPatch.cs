namespace GalaxyWorld.Models.Dbos;

public class StarPatch
{
    public Optional<int?> AthygId { get; set; } = default;
    public Optional<string?> Tycho2Id { get; set; } = default;
    public Optional<long?> GaiaDr3Id { get; set; } = default;
    public Optional<int?> HygV3Id { get; set; } = default;
    public Optional<int?> HipparcosId { get; set; } = default;
    public Optional<int?> HenryDraperId { get; set; } = default;
    public Optional<int?> HarvardYaleId { get; set; } = default;
    public Optional<string?> GlieseId { get; set; } = default;
    public Optional<string?> BayerId { get; set; } = default;
    public Optional<int?> FlamsteedId { get; set; } = default;
    public Optional<string?> Constellation { get; set; } = default;
    public Optional<string?> ProperName { get; set; } = default;
    public Optional<decimal> RightAscension { get; set; } = default;
    public Optional<decimal> Declination { get; set; } = default;
    public Optional<string> PosSrc { get; set; } = default;
    public Optional<decimal?> Distance { get; set; } = default;
    public Optional<decimal?> X0 { get; set; } = default;
    public Optional<decimal?> Y0 { get; set; } = default;
    public Optional<decimal?> Z0 { get; set; } = default;
    public Optional<string?> DistanceSrc { get; set; } = default;
    public Optional<decimal> Magnitude { get; set; } = default;
    public Optional<decimal?> AbsoluteMagnitude { get; set; } = default;
    public Optional<decimal?> ColourIndex { get; set; } = default;
    public Optional<string> MagnitudeSrc { get; set; } = default;
    public Optional<decimal?> RadialVelocity { get; set; } = default;
    public Optional<string?> RadialVelocitySrc { get; set; } = default;
    public Optional<decimal?> ProperMotionRightAscension { get; set; } = default;
    public Optional<decimal?> ProperMotionDeclination { get; set; } = default;
    public Optional<string?> ProperMotionSrc { get; set; } = default;
    public Optional<decimal?> VelocityX { get; set; } = default;
    public Optional<decimal?> VelocityY { get; set; } = default;
    public Optional<decimal?> VelocityZ { get; set; } = default;
    public Optional<string?> SpectralType { get; set; } = default;
    public Optional<string?> SpectralTypeSrc { get; set; } = default;

    private static string MapName(string nameSnake)
    {
        var namePascal = string.Join("", nameSnake.Split('_').Select(n => n.Substring(0, 1).ToUpper() + n.Substring(1)));
        return $"{nameSnake} = @{namePascal},";
    }

    public string ToSql()
    {
        var changes = (AthygId.Map(_ => MapName("athyg_id")).ValueOrDefault ?? "") +
            (Tycho2Id.Map(_ => MapName("tycho2_id")).ValueOrDefault ?? "") +
            (GaiaDr3Id.Map(_ => MapName("gaia_dr3_id")).ValueOrDefault ?? "") +
            (HygV3Id.Map(_ => MapName("hyg_v3_id")).ValueOrDefault ?? "") +
            (HipparcosId.Map(_ => MapName("hipparcos_id")).ValueOrDefault ?? "") +
            (HenryDraperId.Map(_ => MapName("henry_draper_id")).ValueOrDefault ?? "") +
            (HarvardYaleId.Map(_ => MapName("harvard_yale_id")).ValueOrDefault ?? "") +
            (GlieseId.Map(_ => MapName("gliese_id")).ValueOrDefault ?? "") +
            (BayerId.Map(_ => MapName("bayer_id")).ValueOrDefault ?? "") +
            (FlamsteedId.Map(_ => MapName("flamsteed_id")).ValueOrDefault ?? "") +
            (Constellation.Map(_ => MapName("constellation")).ValueOrDefault ?? "") +
            (ProperName.Map(_ => MapName("proper_name")).ValueOrDefault ?? "") +
            (RightAscension.Map(_ => MapName("right_ascension")).ValueOrDefault ?? "") +
            (Declination.Map(_ => MapName("declination")).ValueOrDefault ?? "") +
            (PosSrc.Map(_ => MapName("pos_src")).ValueOrDefault ?? "") +
            (Distance.Map(_ => MapName("distance")).ValueOrDefault ?? "") +
            (X0.Map(_ => MapName("x0")).ValueOrDefault ?? "") +
            (Y0.Map(_ => MapName("y0")).ValueOrDefault ?? "") +
            (Z0.Map(_ => MapName("z0")).ValueOrDefault ?? "") +
            (DistanceSrc.Map(_ => MapName("distance_src")).ValueOrDefault ?? "") +
            (Magnitude.Map(_ => MapName("magnitude")).ValueOrDefault ?? "") +
            (AbsoluteMagnitude.Map(_ => MapName("absolute_magnitude")).ValueOrDefault ?? "") +
            (ColourIndex.Map(_ => MapName("colour_index")).ValueOrDefault ?? "") +
            (MagnitudeSrc.Map(_ => MapName("magnitude_src")).ValueOrDefault ?? "") +
            (RadialVelocity.Map(_ => MapName("radial_velocity")).ValueOrDefault ?? "") +
            (RadialVelocitySrc.Map(_ => MapName("radial_velocity_src")).ValueOrDefault ?? "") +
            (ProperMotionRightAscension.Map(_ => MapName("proper_motion_right_ascension")).ValueOrDefault ?? "") +
            (ProperMotionDeclination.Map(_ => MapName("proper_motion_declination")).ValueOrDefault ?? "") +
            (ProperMotionSrc.Map(_ => MapName("proper_motion_src")).ValueOrDefault ?? "") +
            (VelocityX.Map(_ => MapName("velocity_x")).ValueOrDefault ?? "") +
            (VelocityY.Map(_ => MapName("velocity_y")).ValueOrDefault ?? "") +
            (VelocityZ.Map(_ => MapName("velocity_z")).ValueOrDefault ?? "") +
            (SpectralType.Map(_ => MapName("spectral_type")).ValueOrDefault ?? "") +
            (SpectralTypeSrc.Map(_ => MapName("spectral_type_src")).ValueOrDefault ?? "");
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }
}
