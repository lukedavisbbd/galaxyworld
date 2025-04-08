namespace GalaxyWorld.Models.Catalogue;

public enum CatalogueSort
{
    CatName,
    CatNameDsc,
    CatSlug,
    CatSlugDsc,
}

public static class CatalogueSortExt
{
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
