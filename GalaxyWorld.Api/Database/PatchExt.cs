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
            patch.Constellation.Map(_ => MapName("constellation")).OrDefault() +
            patch.ProperName.Map(_ => MapName("proper_name")).OrDefault() +
            patch.RightAscension.Map(_ => MapName("right_ascension")).OrDefault() +
            patch.Declination.Map(_ => MapName("declination")).OrDefault() +
            patch.PosSrc.Map(_ => MapName("pos_src")).OrDefault() +
            patch.Distance.Map(_ => MapName("distance")).OrDefault() +
            patch.X0.Map(_ => MapName("x0")).OrDefault() +
            patch.Y0.Map(_ => MapName("y0")).OrDefault() +
            patch.Z0.Map(_ => MapName("z0")).OrDefault() +
            patch.DistanceSrc.Map(_ => MapName("distance_src")).OrDefault() +
            patch.Magnitude.Map(_ => MapName("magnitude")).OrDefault() +
            patch.AbsoluteMagnitude.Map(_ => MapName("absolute_magnitude")).OrDefault() +
            patch.ColourIndex.Map(_ => MapName("colour_index")).OrDefault() +
            patch.MagnitudeSrc.Map(_ => MapName("magnitude_src")).OrDefault() +
            patch.RadialVelocity.Map(_ => MapName("radial_velocity")).OrDefault() +
            patch.RadialVelocitySrc.Map(_ => MapName("radial_velocity_src")).OrDefault() +
            patch.ProperMotionRightAscension.Map(_ => MapName("proper_motion_right_ascension")).OrDefault() +
            patch.ProperMotionDeclination.Map(_ => MapName("proper_motion_declination")).OrDefault() +
            patch.ProperMotionSrc.Map(_ => MapName("proper_motion_src")).OrDefault() +
            patch.VelocityX.Map(_ => MapName("velocity_x")).OrDefault() +
            patch.VelocityY.Map(_ => MapName("velocity_y")).OrDefault() +
            patch.VelocityZ.Map(_ => MapName("velocity_z")).OrDefault() +
            patch.SpectralType.Map(_ => MapName("spectral_type")).OrDefault() +
            patch.SpectralTypeSrc.Map(_ => MapName("spectral_type_src")).OrDefault();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this CataloguePatch patch)
    {
        var changes =
            patch.CatName.Map(_ => "cat_name = @CatName,").OrDefault() +
            patch.CatSlug.Map(_ => "cat_slug = @CatSlug,").OrDefault();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this ConstellationPatch patch)
    {
        var changes =
            patch.ConName.Map(_ => "con_name = @ConName,").OrDefault() +
            patch.IauAbbr.Map(_ => "iau_abbr = @IauAbbr,").OrDefault() +
            patch.NasaAbbr.Map(_ => "nasa_abbr = @NasaAbbr,").OrDefault() +
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
