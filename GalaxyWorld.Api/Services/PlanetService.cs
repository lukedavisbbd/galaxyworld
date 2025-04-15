using System.Web;
using GalaxyWorld.Api.Models;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Planets;

namespace GalaxyWorld.API.Services;

public class PlanetService(HttpClient client, CatalogueService catService, CatalogueEntryService catEntryService)
{
    const string BASE_URL = "https://exoplanetarchive.ipac.caltech.edu/TAP/sync?format=json&query=";
    
    public async Task<PlanetarySystem?> GetPlanetarySystem(int starId)
    {
        var pageAll = new Page { Length = int.MaxValue };
        var catalogues = await catService.Get(pageAll, default, []);
        
        var hdCatId = catalogues.FirstOrDefault(cat => cat.CatSlug == "hd")?.CatId;
        var hipCatId = catalogues.FirstOrDefault(cat => cat.CatSlug == "hipparcos")?.CatId;
        var gaiaCatId = catalogues.FirstOrDefault(cat => cat.CatSlug == "gaia3")?.CatId;

        var entries = await catEntryService.GetByStar(starId, pageAll, default, []);

        var hdEntry = entries.FirstOrDefault(entry => entry.CatId == hdCatId);
        var hipEntry = entries.FirstOrDefault(entry => entry.CatId == hipCatId);
        var gaiaEntry = entries.FirstOrDefault(entry => entry.CatId == gaiaCatId);

        var hdId = hdEntry?.EntryId ?? hdEntry?.EntryDesignation;
        var hipId = hipEntry?.EntryId ?? hipEntry?.EntryDesignation;
        var gaiaId = gaiaEntry?.EntryId ?? gaiaEntry?.EntryDesignation;

        var hdSql = mapEntryIdSql(hdId, "HD", "hd_name");
        var hipSql = mapEntryIdSql(hdId, "HIP", "hip_name");
        var gaiaSql = mapEntryIdSql(hdId, "GAIA DR2", "gaia_id");

        List<string?> names = [hdSql, hipSql, gaiaSql, "0 = 1"];
        var nameCond = string.Join(" OR ", names.Where(name => name != null));

        var query = $"SELECT pl_name, soltype, discoverymethod, disc_year, disc_facility, disc_telescope, pl_rade, pl_radj, pl_masse, pl_massj, pl_controv_flag, sy_snum, sy_pnum, sy_mnum FROM ps WHERE default_flag = 1 AND ({nameCond})";
        var url = BASE_URL + HttpUtility.UrlEncode(query);

        var planetRecords = await client.GetFromJsonAsync<List<PlanetRecord>>(url) ?? [];

        if (planetRecords.Count == 0)
            return null;

        var numStars = planetRecords[0].NumStars;
        var numPlanets = planetRecords[0].NumPlanets;
        var numMoons = planetRecords[0].NumMoons;

        var planets = planetRecords.Select(planet => new Planet
        {
            PlanetName = planet.PlanetName,
            SolutionType = planet.SolutionType,
            Controversial = planet.Controverial > 0,
            DiscoveryMethod = planet.DiscoveryMethod,
            DiscYear = planet.DiscYear,
            DiscFacility = planet.DiscFacility,
            DiscTelescope = planet.DiscTelescope,
            RadiusEarth = planet.RadiusEarth,
            RadiusJupiter = planet.RadiusJupiter,
            MassEarth = planet.MassEarth,
            MassJupiter = planet.MassJupiter,
        });

        return new PlanetarySystem {
            NumStars = numStars,
            NumPlanets = numPlanets,
            NumMoons = numMoons,
            Planets = planets.ToList(),
        };
    }

    string? mapEntryIdSql(string? id, string prefix, string columnName)
    {
        id = id != null ? string.Concat(id.Where(char.IsAsciiDigit)) : null;
        if (string.IsNullOrEmpty(id)) return null;
        return $"{columnName} = '{prefix} {id}'";
    }
}
