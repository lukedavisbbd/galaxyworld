using GalaxyWorld.Database;
using GalaxyWorld.Models.Dbos;

namespace GalaxyWorld.Services;

public class UserService(UserRepository repo)
{
    public async Task<User> GetOrCreateUser(string googleId)
    {
        return await repo.FetchOrInsertUser(googleId);
    }

    public async Task<IEnumerable<string>> GetUserRoles(int userId)
    {
        return await repo.FetchUserRoles(userId);
    }
}
