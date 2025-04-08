namespace GalaxyWorld.Models.CatalogueEntry;

public enum CatalogueStarEntrySort
{
    EntryId,
    EntryIdDsc,
}

public static class CatalogueStarEntrySortExt
{
    public static string ToSql(this CatalogueStarEntrySort sort)
    {
        return sort switch
        {
            CatalogueStarEntrySort.EntryId => "ORDER BY entry_id",
            CatalogueStarEntrySort.EntryIdDsc => "ORDER BY entry_id DESC",
            _ => "",
        };
    }
}
