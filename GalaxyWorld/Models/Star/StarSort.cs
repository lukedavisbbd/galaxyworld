namespace GalaxyWorld.Models.Star;

public enum StarSort
{
    ProperName,
    ProperNameDsc,
    Distance,
    DistanceDsc,
    Magnitude,
    MagnitudeDsc,
    AbsoluteMagnitude,
    AbsoluteMagnitudeDsc,
}

public static class StarSortExt
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
}
