namespace GalaxyWorld.Models.Constellation;

public class ConstellationPatch
{
    public Optional<string> ConName { get; init; } = default;
    public Optional<string> IauAbbr { get; init; } = default;
    public Optional<string> NasaAbbr { get; init; } = default;
    public Optional<string> Genitive { get; init; } = default;
    public Optional<string> Origin { get; init; } = default;
    public Optional<string> Meaning { get; init; } = default;

    public string ToSql()
    {
        var changes =
            ConName.Map(_ => "con_name = @ConName,").Or() +
            IauAbbr.Map(_ => "iau_abbr = @IauAbbr,").Or() +
            NasaAbbr.Map(_ => "nasa_abbr = @NasaAbbr,").Or() +
            Genitive.Map(_ => "genitive = @Genitive,").Or() +
            Origin.Map(_ => "origin = @Origin,").Or() +
            Meaning.Map(_ => "meaning = @Meaning,").Or();
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }
}
