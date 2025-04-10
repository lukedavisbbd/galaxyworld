namespace GalaxyWorld.Core.Models.Star;

public class StarPatch
{
    public Optional<int?> Constellation { get; init; } = default;
    public Optional<string?> ProperName { get; init; } = default;
    public Optional<double> RightAscension { get; init; } = default;
    public Optional<double> Declination { get; init; } = default;
    public Optional<string> PosSrc { get; init; } = default;
    public Optional<double?> Distance { get; init; } = default;
    public Optional<double?> X0 { get; init; } = default;
    public Optional<double?> Y0 { get; init; } = default;
    public Optional<double?> Z0 { get; init; } = default;
    public Optional<string?> DistanceSrc { get; init; } = default;
    public Optional<double> Magnitude { get; init; } = default;
    public Optional<double?> AbsoluteMagnitude { get; init; } = default;
    public Optional<double?> ColourIndex { get; init; } = default;
    public Optional<string> MagnitudeSrc { get; init; } = default;
    public Optional<double?> RadialVelocity { get; init; } = default;
    public Optional<string?> RadialVelocitySrc { get; init; } = default;
    public Optional<double?> ProperMotionRightAscension { get; init; } = default;
    public Optional<double?> ProperMotionDeclination { get; init; } = default;
    public Optional<string?> ProperMotionSrc { get; init; } = default;
    public Optional<double?> VelocityX { get; init; } = default;
    public Optional<double?> VelocityY { get; init; } = default;
    public Optional<double?> VelocityZ { get; init; } = default;
    public Optional<string?> SpectralType { get; init; } = default;
    public Optional<string?> SpectralTypeSrc { get; init; } = default;
}
