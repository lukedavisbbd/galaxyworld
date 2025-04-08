﻿using System.Security.Claims;
using GalaxyWorld.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace GalaxyWorld.Auth;

public class RoleRequirementHandler(UserService service, IMemoryCache cache) : AuthorizationHandler<RoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var googleId = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (googleId == null)
        {
            context.Fail();
            return;
        }

        var user = await cache.GetOrCreateAsync("user:google_id:" + googleId, async cache =>
        {
            cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await service.GetOrCreate(googleId.Value);
        }) ?? throw new InvalidOperationException("user not found or able to be created or cached");

        var roles = await cache.GetOrCreateAsync("roles:user_id:" + user.UserId, async cache =>
        {
            cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await service.GetRoles(user.UserId);
        }) ?? throw new InvalidOperationException("user roles not found or able to be cached");

        if (!roles.Contains(requirement.RoleName))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
