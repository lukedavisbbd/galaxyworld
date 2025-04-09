using GalaxyWorld.API.Database;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Constellation;

namespace GalaxyWorld.API.Services;

public class ConstellationService(ConstellationRepository repo)
{
    public async Task<IEnumerable<Constellation>> Get(Page page, ConstellationSort sort)
    {
        return await repo.Fetch(page, sort);
    }

    public async Task<Constellation?> GetOne(int id)
    {
        return await repo.FetchOne(id);
    }

    public async Task<Constellation> Create(ConstellationInsert insert)
    {
        return await repo.Insert(insert);
    }

    public async Task<Constellation?> Patch(int id, ConstellationPatch patch)
    {
        return await repo.Update(id, patch);
    }

    public async Task<Constellation?> Delete(int id)
    {
        return await repo.Delete(id);
    }
}
