using Dapper;
using GalaxyWorld.Models.Dbos;

namespace GalaxyWorld.Database;

public class UserRepository(DbContext db)
{
    public async Task<User> FetchOrInsertUser(string googleId)
    {
        var conn = db.Connection;
        var user = await conn.QueryFirstAsync<User>(
            "INSERT INTO users(google_id) VALUES (@googleId) " +
            "ON CONFLICT(google_id) DO UPDATE SET last_logged_in = now() RETURNING *", new
            {
                googleId,
            });
        return user;
    }

    public async Task<IEnumerable<string>> FetchUserRoles(int userId)
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
