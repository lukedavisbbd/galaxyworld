namespace GalaxyWorld.API.Database;

public static class DbConstants
{
    private static IDictionary<string, string> ConstraintNames = new Dictionary<string, string> {
        { "users_google_id_check", "User Google ID is invalid." },
        { "roles_role_name_check", "Role name may not be empty or start/end with whitespace." },

        { "stars_constellation_fkey", "Constellation does not exist." },
        { "stars_proper_name_check", "Star name may not be empty or start/end with whitespace." },
        { "stars_spectral_type_check", "Star spectral type may not be empty or start/end with whitespace." },

        { "catalogues_catalogue_name_key", "Category name already taken." },
        { "catalogues_catalogue_name_check", "Category name may not be empty or start/end with whitespace." },
        { "catalogues_catalogue_slug_key", "Category slug already taken." },
        { "catalogues_catalogue_slug_check", "Category slug may only contain lowercase alphanumeric characters and underscores." },

        { "constellations_constellation_name_key", "Constellation name already taken." },
        { "constellations_constellation_name_check", "Constellation name may not be empty or start/end with whitespace." },
        { "constellations_iau_abbreviation_key", "Constellation IAU abbreviation already taken." },
        { "constellations_iau_abbreviation_check", "Constellation IAU abbreviation must be exactly 3 letters." },
        { "constellations_nasa_abbreviation_key", "Constellation NASA abbreviation already taken." },
        { "constellations_nasa_abbreviation_check", "Constellation NASA abbreviation must be exactly 4 letters." },
        
        { "catalogue_entries_pkey", "Star already in category." },
        { "catalogue_entries_catalogue_id_fkey", "Category does not exist." },
        { "catalogue_entries_star_id_fkey", "Star does not exist." },
        { "catalogue_entries_entry_id_check", "Entry ID may not be empty or start/end with whitespace." },
        { "catalogue_entries_entry_designation_check", "Entry designation may not be empty or start/end with whitespace." },
        { "catalogue_entries_catalogue_id_entry_id_key", "Entry ID already taken." },
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
            constraint = constraint.Substring(0, constraint.Length - 5) + " already exists";

        if (constraint.EndsWith(" fkey"))
            constraint = constraint.Substring(0, constraint.Length - 5) + " mismatch";

        if (constraint.EndsWith(" check"))
            constraint = constraint.Substring(0, constraint.Length - 6) + " is invalid";

        if (constraint.EndsWith(" null"))
            constraint = constraint.Substring(0, constraint.Length - 5) + " must be set";

        if (constraint.EndsWith(" invalid"))
            constraint = constraint.Substring(0, constraint.Length - 8) + " is invalid";

        if (constraint.EndsWith(" long"))
            constraint = constraint.Substring(0, constraint.Length - 5) + " is too long";

        constraint = constraint.Substring(0, 1).ToUpper() + constraint.Substring(1) + '.';

        return constraint;
    }

    public static string MapPropertyName(string pascalCase)
    {
        var snakeCase = "";

        foreach (var c in pascalCase.ToCharArray())
        {
            if (char.IsUpper(c))
                snakeCase += '_';
            
            snakeCase += char.ToLower(c);
        }

        return snakeCase.TrimStart('_');
    }
}
