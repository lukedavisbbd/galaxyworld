namespace GalaxyWorld.Services;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
            .AddScoped<StarService>()
            .AddScoped<UserService>();
    }
}
