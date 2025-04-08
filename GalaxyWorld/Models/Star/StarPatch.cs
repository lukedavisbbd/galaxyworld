namespace GalaxyWorld.Models.Star;

public class StarPatch
{
    public Optional<int?> Constellation { get; init; } = default;
    public Optional<string?> ProperName { get; init; } = default;
    public Optional<decimal> RightAscension { get; init; } = default;
    public Optional<decimal> Declination { get; init; } = default;
    public Optional<string> PosSrc { get; init; } = default;
    public Optional<decimal?> Distance { get; init; } = default;
    public Optional<decimal?> X0 { get; init; } = default;
    public Optional<decimal?> Y0 { get; init; } = default;
    public Optional<decimal?> Z0 { get; init; } = default;
    public Optional<string?> DistanceSrc { get; init; } = default;
    public Optional<decimal> Magnitude { get; init; } = default;
    public Optional<decimal?> AbsoluteMagnitude { get; init; } = default;
    public Optional<decimal?> ColourIndex { get; init; } = default;
    public Optional<string> MagnitudeSrc { get; init; } = default;
    public Optional<decimal?> RadialVelocity { get; init; } = default;
    public Optional<string?> RadialVelocitySrc { get; init; } = default;
    public Optional<decimal?> ProperMotionRightAscension { get; init; } = default;
    public Optional<decimal?> ProperMotionDeclination { get; init; } = default;
    public Optional<string?> ProperMotionSrc { get; init; } = default;
    public Optional<decimal?> VelocityX { get; init; } = default;
    public Optional<decimal?> VelocityY { get; init; } = default;
    public Optional<decimal?> VelocityZ { get; init; } = default;
    public Optional<string?> SpectralType { get; init; } = default;
    public Optional<string?> SpectralTypeSrc { get; init; } = default;

    private static string MapName(string nameSnake)
    {
        var namePascal = string.Join("", nameSnake.Split('_').Select(n => n.Substring(0, 1).ToUpper() + n.Substring(1)));
        return $"{nameSnake} = @{namePascal},";
    }

    public string ToSql()
    {
        var changes = 
            Constellation.Map(_ => MapName("constellation")).Or() +
            ProperName.Map(_ => MapName("proper_name")).Or() +
            RightAscension.Map(_ => MapName("right_ascension")).Or() +
            Declination.Map(_ => MapName("declination")).Or() +
            PosSrc.Map(_ => MapName("pos_src")).Or() +
            Distance.Map(_ => MapName("distance")).Or() +
            X0.Map(_ => MapName("x0")).Or() +
            Y0.Map(_ => MapName("y0")).Or() +
            Z0.Map(_ => MapName("z0")).Or() +
            DistanceSrc.Map(_ => MapName("distance_src")).Or() +
            Magnitude.Map(_ => MapName("magnitude")).Or() +
            AbsoluteMagnitude.Map(_ => MapName("absolute_magnitude")).Or() +
            ColourIndex.Map(_ => MapName("colour_index")).Or() +
            MagnitudeSrc.Map(_ => MapName("magnitude_src")).Or() +
            RadialVelocity.Map(_ => MapName("radial_velocity")).Or() +
            RadialVelocitySrc.Map(_ => MapName("radial_velocity_src")).Or() +
            ProperMotionRightAscension.Map(_ => MapName("proper_motion_right_ascension")).Or() +
            ProperMotionDeclination.Map(_ => MapName("proper_motion_declination")).Or() +
            ProperMotionSrc.Map(_ => MapName("proper_motion_src")).Or() +
            VelocityX.Map(_ => MapName("velocity_x")).Or() +
            VelocityY.Map(_ => MapName("velocity_y")).Or() +
            VelocityZ.Map(_ => MapName("velocity_z")).Or() +
            SpectralType.Map(_ => MapName("spectral_type")).Or() +
            SpectralTypeSrc.Map(_ => MapName("spectral_type_src")).Or();
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }
}
