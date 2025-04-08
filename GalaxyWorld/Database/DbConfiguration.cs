using GalaxyWorld.Models.CatalogueEntry;

namespace GalaxyWorld.Database;

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
            .AddScoped<CatalogueStarEntryRepository>()
            .AddScoped<ConstellationRepository>();
    }
}
