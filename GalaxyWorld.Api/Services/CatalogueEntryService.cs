using GalaxyWorld.API.Database;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.API.Services;

public class CatalogueEntryService(CatalogueEntryRepository repo)
{
    public async Task<IEnumerable<CatalogueEntry>> GetByStar(int star, Page page, CatalogueEntrySort sort)
    {
        return await repo.FetchByStar(star, page, sort);
    }

    public async Task<IEnumerable<CatalogueEntry>> GetByCatalogue(int catId, Page page, CatalogueEntrySort sort)
    {
        return await repo.FetchByCatalogue(catId, page, sort);
    }

    public async Task<CatalogueEntry?> GetOne(int catId, int starId)
    {
        return await repo.FetchOne(catId, starId);
    }

    public async Task<CatalogueEntry> Create(int catId, int starId, CatalogueEntryInsert insert)
    {
        return await repo.Insert(catId, starId, insert);
    }

    public async Task<CatalogueEntry?> Patch(int catId, int starId, CatalogueEntryPatch patch)
    {
        return await repo.Update(catId, starId, patch);
    }

    public async Task<CatalogueEntry?> Delete(int catId, int starId)
    {
        return await repo.Delete(catId, starId);
    }
}
