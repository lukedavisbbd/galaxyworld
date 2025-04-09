using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace GalaxyWorld.API.Auth;

public static class AuthConfiguration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<AuthOptions>();
        
        var options = config.GetRequiredSection(AuthOptions.Section).Get<AuthOptions>()
            ?? throw new InvalidOperationException("missing google jwt options");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt => jwt.UseGoogle(options.ClientId));

        services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();

        services.AddAuthorization(auth =>
        {
            auth.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            auth.AddPolicy(RoleRequirement.RoleAdmin.PolicyName, policy => policy.AddRequirements(RoleRequirement.RoleAdmin));
        });

        return services;
    }
}
