namespace GalaxyWorld.API.Services;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient<AuthService>();
        return services
            .AddScoped<UserService>()
            .AddScoped<StarService>()
            .AddScoped<CatalogueService>()
            .AddScoped<CatalogueEntryService>()
            .AddScoped<ConstellationService>();
    }
}
