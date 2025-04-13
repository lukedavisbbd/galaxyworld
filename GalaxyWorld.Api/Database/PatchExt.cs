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
            patch.Constellation.Map(_ => MapName("constellation")).Or() +
            patch.ProperName.Map(_ => MapName("proper_name")).Or() +
            patch.RightAscension.Map(_ => MapName("right_ascension")).Or() +
            patch.Declination.Map(_ => MapName("declination")).Or() +
            patch.PosSrc.Map(_ => MapName("pos_src")).Or() +
            patch.Distance.Map(_ => MapName("distance")).Or() +
            patch.X0.Map(_ => MapName("x0")).Or() +
            patch.Y0.Map(_ => MapName("y0")).Or() +
            patch.Z0.Map(_ => MapName("z0")).Or() +
            patch.DistanceSrc.Map(_ => MapName("distance_src")).Or() +
            patch.Magnitude.Map(_ => MapName("magnitude")).Or() +
            patch.AbsoluteMagnitude.Map(_ => MapName("absolute_magnitude")).Or() +
            patch.ColourIndex.Map(_ => MapName("colour_index")).Or() +
            patch.MagnitudeSrc.Map(_ => MapName("magnitude_src")).Or() +
            patch.RadialVelocity.Map(_ => MapName("radial_velocity")).Or() +
            patch.RadialVelocitySrc.Map(_ => MapName("radial_velocity_src")).Or() +
            patch.ProperMotionRightAscension.Map(_ => MapName("proper_motion_right_ascension")).Or() +
            patch.ProperMotionDeclination.Map(_ => MapName("proper_motion_declination")).Or() +
            patch.ProperMotionSrc.Map(_ => MapName("proper_motion_src")).Or() +
            patch.VelocityX.Map(_ => MapName("velocity_x")).Or() +
            patch.VelocityY.Map(_ => MapName("velocity_y")).Or() +
            patch.VelocityZ.Map(_ => MapName("velocity_z")).Or() +
            patch.SpectralType.Map(_ => MapName("spectral_type")).Or() +
            patch.SpectralTypeSrc.Map(_ => MapName("spectral_type_src")).Or();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this CataloguePatch patch)
    {
        var changes =
            patch.CatName.Map(_ => "cat_name = @CatName,").Or() +
            patch.CatSlug.Map(_ => "cat_slug = @CatSlug,").Or();
        return changes.TrimEnd(',');
    }

    public static string ToSql(this ConstellationPatch patch)
    {
        var changes =
            patch.ConName.Map(_ => "con_name = @ConName,").Or() +
            patch.IauAbbr.Map(_ => "iau_abbr = @IauAbbr,").Or() +
            patch.NasaAbbr.Map(_ => "nasa_abbr = @NasaAbbr,").Or() +
            patch.Genitive.Map(_ => "genitive = @Genitive,").Or() +
            patch.Origin.Map(_ => "origin = @Origin,").Or() +
            patch.Meaning.Map(_ => "meaning = @Meaning,").Or();
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }

    public static string ToSql(this CatalogueEntryPatch patch)
    {
        var changes =
            patch.EntryId.Map(_ => "entry_id = @EntryId,").Or() +
            patch.EntryDesignation.Map(_ => "entry_designation = @EntryDesignation,").Or();
        return changes.TrimEnd(',');
    }
}
