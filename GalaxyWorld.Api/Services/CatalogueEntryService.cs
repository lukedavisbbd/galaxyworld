using GalaxyWorld.API.Database;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.API.Services;

public class CatalogueEntryService(CatalogueEntryRepository repo)
{
    public async Task<IEnumerable<CatalogueEntry>> GetByStar(int star, Page page, CatalogueEntrySort sort, Filter<CatalogueEntry>[] filters)
    {
        return await repo.FetchByStar(star, page, sort, filters);
    }

    public async Task<IEnumerable<CatalogueEntry>> GetByCatalogue(int catalogueId, Page page, CatalogueEntrySort sort, Filter<CatalogueEntry>[] filters)
    {
        return await repo.FetchByCatalogue(catalogueId, page, sort, filters);
    }

    public async Task<CatalogueEntry?> GetOne(int catalogueId, int starId)
    {
        return await repo.FetchOne(catalogueId, starId);
    }

    public async Task<CatalogueEntry> Create(int catalogueId, int starId, CatalogueEntryInsert insert)
    {
        return await repo.Insert(catalogueId, starId, insert);
    }

    public async Task<CatalogueEntry?> Patch(int catalogueId, int starId, CatalogueEntryPatch patch)
    {
        return await repo.Update(catalogueId, starId, patch);
    }

    public async Task<CatalogueEntry?> Delete(int catalogueId, int starId)
    {
        return await repo.Delete(catalogueId, starId);
    }
}
