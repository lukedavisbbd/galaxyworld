using Microsoft.AspNetCore.Authorization;

namespace GalaxyWorld.API.Auth;

public record RoleRequirement(string RoleName) : IAuthorizationRequirement
{
    public string PolicyName => "ROLE_" + RoleName.ToUpper();

    public static RoleRequirement RoleAdmin = new RoleRequirement("admin");
}
