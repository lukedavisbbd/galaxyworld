﻿namespace GalaxyWorld.Services;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
            .AddScoped<UserService>()
            .AddScoped<StarService>()
            .AddScoped<CatalogueService>()
            .AddScoped<CatalogueStarEntryService>()
            .AddScoped<ConstellationService>();
    }
}
