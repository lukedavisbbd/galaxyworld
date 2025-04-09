namespace GalaxyWorld.Core.Models.Star;

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
}
