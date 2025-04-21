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
            _ => "",
        };
    }

    public static string ToSql(this ConstellationSort sort)
    {
        return sort switch
        {
            ConstellationSort.ConstellationName => "ORDER BY constellation_name",
            ConstellationSort.ConstellationNameDsc => "ORDER BY constellation_name DESC",
            ConstellationSort.IauAbbreviation => "ORDER BY iau_abbreviation",
            ConstellationSort.IauAbbreviationDsc => "ORDER BY iau_abbreviation DESC",
            ConstellationSort.NasaAbbreviation => "ORDER BY nasa_abbreviation",
            ConstellationSort.NasaAbbreviationDsc => "ORDER BY nasa_abbreviation DESC",
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
            CatalogueSort.CatalogueName => "ORDER BY catalogue_name",
            CatalogueSort.CatalogueNameDsc => "ORDER BY catalogue_name DESC",
            CatalogueSort.CatalogueSlug => "ORDER BY catalogue_slug",
            CatalogueSort.CatalogueSlugDsc => "ORDER BY catalogue_slug DESC",
            _ => "",
        };
    }
}
