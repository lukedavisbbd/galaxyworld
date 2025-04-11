using GalaxyWorld.API.Database;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Catalogue;

namespace GalaxyWorld.API.Services;

public class CatalogueService(CatalogueRepository repo)
{
    public async Task<IEnumerable<Catalogue>> Get(Page page, CatalogueSort sort, Filter<Catalogue>[] filters)
    {
        return await repo.Fetch(page, sort, filters);
    }

    public async Task<Catalogue?> GetOne(int id)
    {
        return await repo.FetchOne(id);
    }

    public async Task<Catalogue> Create(CatalogueInsert insert)
    {
        return await repo.Insert(insert);
    }

    public async Task<Catalogue?> Patch(int id, CataloguePatch patch)
    {
        return await repo.Update(id, patch);
    }

    public async Task<Catalogue?> Delete(int id)
    {
        return await repo.Delete(id);
    }
}
