namespace GalaxyWorld.API.Database;

public static class DbConstants
{
    private static IDictionary<string, string> ConstraintNames = new Dictionary<string, string> {
        { "stars_constellation_fkey", "Constellation does not exist." },

        { "catalogues_cat_name_key", "Category name already taken." },
        { "catalogues_cat_slug_key", "Category slug already taken." },
        { "catalogues_cat_slug_check", "Category slug may only contain lowercase alphanumeric characters and underscores." },

        { "constellations_con_name_key", "Constellation name already taken." },
        { "constellations_iau_abbr_key", "Constellation IAU abbreviation already taken." },
        { "constellations_iau_abbr_check", "Constellation IAU abbreviation must be exactly 3 letters." },
        { "constellations_nasa_abbr_key", "Constellation NASA abbreviation already taken." },
        { "constellations_nasa_abbr_check", "Constellation NASA abbreviation must be exactly 4 letters." },
        
        { "catalogue_entries_pkey", "Star already in category." },
        { "catalogue_entries_cat_id_fkey", "Category does not exist." },
        { "catalogue_entries_star_id_fkey", "Star does not exist." },
        { "catalogue_entries_entry_id_key", "Entry ID already taken." },
        { "catalogue_entries_check", "Entry must have either a unique ID or a designation." },
    };

    public static string MapConstraintName(string constraint)
    {
        if (ConstraintNames.ContainsKey(constraint))
            return ConstraintNames[constraint];

        constraint = constraint.Replace('_', ' ');

        if (constraint.EndsWith(" key"))
            constraint = constraint.Substring(0, constraint.Length - 4) + " already taken";

        if (constraint.EndsWith(" pkey"))
            constraint = constraint.Substring(0, constraint.Length - 4) + " already exists";

        if (constraint.EndsWith(" fkey"))
            constraint = constraint.Substring(0, constraint.Length - 4) + " mismatch";

        constraint = constraint.Substring(0, 1).ToUpper() + constraint.Substring(1) + '.';

        return constraint;
    }
}
