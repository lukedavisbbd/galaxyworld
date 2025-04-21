using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.API.Database;

public static class PatchExt
{
    private static string MapName(string nameSnake)
    {
        var namePascal = string.Join("", nameSnake.Split('_').Select(n => n.Substring(0, 1).ToUpper() + n.Substring(1)));
        return $"{nameSnake} = @{namePascal},";
    }

    public static string ToSql(this StarPatch patch)
    {
        var changes =
            patch.ConstellationId.Map(_ => MapName("constellation_id")).OrDefault() +
            patch.ProperName.Map(_ => MapName("proper_name")).OrDefault() +
            patch.RightAscension.Map(_ => MapName("right_ascension")).OrDefault() +
            patch.Declination.Map(_ => MapName("declination")).OrDefault() +
            patch.Distance.Map(_ => MapName("distance")).OrDefault() +
            patch.PositionCartesian.Map(_ => MapName("position_cartesian")).OrDefault() +
            patch.Magnitude.Map(_ => MapName("magnitude")).OrDefault() +
            patch.ColourIndex.Map(_ => MapName("colour_index")).OrDefault() +
            patch.VelocityCircular.Map(_ => MapName("velocity_circular")).OrDefault() +
            patch.VelocityCartesian.Map(_ => MapName("velocity_cartesian")).OrDefault() +
            patch.ColourIndex.Map(_ => MapName("colour_index")).OrDefault() +
            patch.SpectralType.Map(_ => MapName("spectral_type")).OrDefault();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this CataloguePatch patch)
    {
        var changes =
            patch.CatalogueName.Map(_ => "catalogue_name = @CatalogueName,").OrDefault() +
            patch.CatalogueSlug.Map(_ => "catalogue_slug = @CatalogueSlug,").OrDefault();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this ConstellationPatch patch)
    {
        var changes =
            patch.ConstellationName.Map(_ => "constellation_name = @ConstellationName,").OrDefault() +
            patch.IauAbbreviation.Map(_ => "iau_abbreviation = @IauAbbreviation,").OrDefault() +
            patch.NasaAbbreviation.Map(_ => "nasa_abbreviation = @NasaAbbreviation,").OrDefault() +
            patch.Genitive.Map(_ => "genitive = @Genitive,").OrDefault() +
            patch.Origin.Map(_ => "origin = @Origin,").OrDefault() +
            patch.Meaning.Map(_ => "meaning = @Meaning,").OrDefault();
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }

    public static string ToSql(this CatalogueEntryPatch patch)
    {
        var changes =
            patch.EntryId.Map(_ => "entry_id = @EntryId,").OrDefault() +
            patch.EntryDesignation.Map(_ => "entry_designation = @EntryDesignation,").OrDefault();
        return changes.TrimEnd(',');
    }
}
