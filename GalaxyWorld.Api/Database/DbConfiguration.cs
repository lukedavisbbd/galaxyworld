namespace GalaxyWorld.API.Database;

public static class DbConfiguration
{
    public static IServiceCollection ConfigureDb(this IServiceCollection services, IConfiguration config)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Dapper.SqlMapper.AddTypeHandler(new VectorMapper());

        services.AddOptions<DbOptions>().Bind(config);
        return services.AddScoped<DbContext>()
            .AddScoped<UserRepository>()
            .AddScoped<StarRepository>()
            .AddScoped<CatalogueRepository>()
            .AddScoped<CatalogueEntryRepository>()
            .AddScoped<ConstellationRepository>();
    }
}
