using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.API.Database;

public static class SortExt
{
    public static string ToSql(this StarSort sort)
    {
        return sort switch
        {
            StarSort.ProperName => "ORDER BY proper_name",
            StarSort.ProperNameDsc => "ORDER BY proper_name DESC",
            StarSort.Distance => "ORDER BY distance",
            StarSort.DistanceDsc => "ORDER BY distance DESC",
            StarSort.Magnitude => "ORDER BY magnitude",
            StarSort.MagnitudeDsc => "ORDER BY magnitude DESC",
            StarSort.AbsoluteMagnitude => "ORDER BY absolute_magnitude",
            StarSort.AbsoluteMagnitudeDsc => "ORDER BY absolute_magnitude DESC",
            _ => "",
        };
    }

    public static string ToSql(this ConstellationSort sort)
    {
        return sort switch
        {
            ConstellationSort.ConName => "ORDER BY con_name",
            ConstellationSort.ConNameDsc => "ORDER BY con_name DESC",
            ConstellationSort.IauAbbr => "ORDER BY iau_abbr",
            ConstellationSort.IauAbbrDsc => "ORDER BY iau_abbr DESC",
            ConstellationSort.NasaAbbr => "ORDER BY nasa_abbr",
            ConstellationSort.NasaAbbrDsc => "ORDER BY nasa_abbr DESC",
            ConstellationSort.Genitive => "ORDER BY genitive",
            ConstellationSort.GenitiveDsc => "ORDER BY genitive DESC",
            _ => "",
        };
    }

    public static string ToSql(this CatalogueEntrySort sort)
    {
        return sort switch
        {
            CatalogueEntrySort.EntryId => "ORDER BY entry_id",
            CatalogueEntrySort.EntryIdDsc => "ORDER BY entry_id DESC",
            CatalogueEntrySort.EntryDesignation => "ORDER BY entry_designation",
            CatalogueEntrySort.EntryDesignationDsc => "ORDER BY entry_designation DESC",
            _ => "",
        };
    }

    public static string ToSql(this CatalogueSort sort)
    {
        return sort switch
        {
            CatalogueSort.CatName => "ORDER BY cat_name",
            CatalogueSort.CatNameDsc => "ORDER BY cat_name DESC",
            CatalogueSort.CatSlug => "ORDER BY cat_slug",
            CatalogueSort.CatSlugDsc => "ORDER BY cat_slug DESC",
            _ => "",
        };
    }
}
