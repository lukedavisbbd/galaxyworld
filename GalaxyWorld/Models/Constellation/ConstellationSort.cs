namespace GalaxyWorld.Models.Constellation;

public enum ConstellationSort
{
    ConName,
    ConNameDsc,
    IauAbbr,
    IauAbbrDsc,
    NasaAbbr,
    NasaAbbrDsc,
    Genitive,
    GenitiveDsc,
}

public static class ConstellationSortExt
{
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
}
