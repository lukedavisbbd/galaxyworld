using GalaxyWorld.API.Database;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.API.Services;

public class StarService(StarRepository repo)
{
    public async Task<IEnumerable<Star>> Get(Page page, StarSort sort)
    {
        return await repo.Fetch(page, sort);
    }

    public async Task<IEnumerable<Star>> GetByConstellation(int constellation, Page page, StarSort sort)
    {
        return await repo.FetchByConstellation(constellation, page, sort);
    }

    public async Task<Star?> GetOne(int id)
    {
        return await repo.FetchOne(id);
    }

    public async Task<Star> Create(StarInsert insert)
    {
        return await repo.Insert(insert);
    }

    public async Task<Star?> Patch(int id, StarPatch patch)
    {
        return await repo.Update(id, patch);
    }

    public async Task<Star?> Delete(int id)
    {
        return await repo.Delete(id);
    }
}
