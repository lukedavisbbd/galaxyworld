using GalaxyWorld.Database;
using GalaxyWorld.Models;
using GalaxyWorld.Models.Dbos;

namespace GalaxyWorld.Services;

public class StarService(StarRepository repo)
{
    public async Task<IEnumerable<Star>> GetStars(Page page)
    {
        return await repo.FetchStars(page);
    }

    public async Task<Star?> GetStar(int id)
    {
        return await repo.FetchStarById(id);
    }

    public async Task<Star> CreateStar(StarInsert insert)
    {
        return await repo.InsertStar(insert);
    }

    public async Task<Star?> PatchStar(int id, StarPatch patch)
    {
        return await repo.UpdateStar(id, patch);
    }
}
