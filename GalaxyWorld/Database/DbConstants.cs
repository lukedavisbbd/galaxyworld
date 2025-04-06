namespace GalaxyWorld.Database;

public static class DbConstants
{
    private static IDictionary<string, string> ConstraintNames = new Dictionary<string, string> {
        { "stars_athyg_id_key", "ATHYG ID already taken." },
        { "stars_bayer_id_key", "Bayer ID already taken." },
        { "stars_flamsteed_id_key", "Flamsteed ID already taken." },
        { "stars_gaia_dr3_id_key", "Gaia DR 3 ID already taken." },
        { "stars_gliese_id_key", "Gliese ID already taken." },
        { "stars_harvard_yale_id_key", "Harvard/Yale ID already taken." },
        { "stars_henry_draper_id_key", "Henry Draper ID already taken." },
        { "stars_hipparcos_id_key", "Hipparcos ID already taken." },
        { "stars_hyg_v3_id_key", "Hyg V3 ID already taken." },
        { "stars_proper_name_key", "proper name already taken." },
        { "stars_tycho2_id_key", "Tycho-2 ID already taken." },
    };

    public static string MapConstraintName(string constraint)
    {
        if (ConstraintNames.ContainsKey(constraint))
            return ConstraintNames[constraint];

        constraint = constraint.Replace('_', ' ');

        if (constraint.EndsWith(" key"))
            constraint = constraint.Substring(0, constraint.Length - 4) + " already taken";

        constraint = constraint.Substring(0, 1).ToUpper() + constraint.Substring(1) + '.';

        return constraint;
    }
}
