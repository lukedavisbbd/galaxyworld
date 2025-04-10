using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.API.Database;

public static class DbConfiguration
{
    public static IServiceCollection ConfigureDb(this IServiceCollection services, IConfiguration config)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddOptions<DbOptions>().Bind(config);
        return services.AddScoped<DbContext>()
            .AddScoped<UserRepository>()
            .AddScoped<StarRepository>()
            .AddScoped<CatalogueRepository>()
            .AddScoped<CatalogueEntryRepository>()
            .AddScoped<ConstellationRepository>();
    }
}
