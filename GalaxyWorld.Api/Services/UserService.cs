using GalaxyWorld.API.Database;
using GalaxyWorld.API.Models;

namespace GalaxyWorld.API.Services;

public class UserService(UserRepository repo)
{
    public async Task<User> GetOrCreate(string googleId)
    {
        return await repo.FetchOrInsert(googleId);
    }

    public async Task<IEnumerable<string>> GetRoles(int userId)
    {
        return await repo.FetchRoles(userId);
    }
}
