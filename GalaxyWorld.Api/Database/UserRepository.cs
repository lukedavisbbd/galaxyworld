using Dapper;
using GalaxyWorld.API.Models;

namespace GalaxyWorld.API.Database;

public class UserRepository(DbContext db)
{
    public async Task<User> FetchOrInsert(string googleId)
    {
        var conn = db.Connection;
        var user = await conn.QueryFirstAsync<User>(
            "INSERT INTO users(google_id) VALUES (@googleId) " +
            "ON CONFLICT(google_id) DO UPDATE SET user_id = users.user_id RETURNING *", new
            {
                googleId,
            });
        return user;
    }

    public async Task<IEnumerable<string>> FetchRoles(int userId)
    {
        var conn = db.Connection;
        var roles = await conn.QueryAsync<string>(
            "SELECT role_name FROM roles NATURAL INNER JOIN user_roles NATURAL INNER JOIN users WHERE user_id = @userId", new
            {
                userId,
            });
        return roles;
    }
}
