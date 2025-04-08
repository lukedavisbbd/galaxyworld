using GalaxyWorld.Database;
using GalaxyWorld.Models;
using GalaxyWorld.Models.CatalogueEntry;

namespace GalaxyWorld.Services;

public class CatalogueStarEntryService(CatalogueStarEntryRepository repo)
{
    public async Task<IEnumerable<CatalogueStarEntry>> GetByStar(int star, Page page, CatalogueStarEntrySort sort)
    {
        return await repo.FetchByStar(star, page, sort);
    }

    public async Task<IEnumerable<CatalogueStarEntry>> GetByCatalogue(int catId, Page page, CatalogueStarEntrySort sort)
    {
        return await repo.FetchByCatalogue(catId, page, sort);
    }

    public async Task<CatalogueStarEntry?> GetOneByEntryId(int catId, string entryId)
    {
        return await repo.FetchOneByEntryId(catId, entryId);
    }

    public async Task<CatalogueStarEntry> Create(CatalogueStarEntry insert)
    {
        return await repo.Insert(insert);
    }

    public async Task<CatalogueStarEntry?> Patch(CatalogueStarEntry patch)
    {
        return await repo.Update(patch);
    }

    public async Task<CatalogueStarEntry?> DeleteByCatelogue(int catId, string entryId)
    {
        return await repo.DeleteByCatelogue(catId, entryId);
    }

    public async Task<CatalogueStarEntry?> DeleteByStar(int starId, int catId)
    {
        return await repo.DeleteByStar(starId, catId);
    }
}
