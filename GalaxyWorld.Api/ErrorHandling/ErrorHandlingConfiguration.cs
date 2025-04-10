using GalaxyWorld.API.Database;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GalaxyWorld.API.ErrorHandling;

public static class ErrorHandlingConfiguration
{
    private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

    public static IServiceCollection ConfigureErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.Map<PostgresException>(ex =>
            {
                var logger = loggerFactory.CreateLogger<Program>();
                if (ex.ConstraintName != null)
                {
                    logger.LogError($"DB query failed constraint '{ex.ConstraintName}': {ex}");
                    return new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Constraint Failed",
                        Detail = DbConstants.MapConstraintName(ex.ConstraintName),
                    };
                }
                logger.LogError($"DB query failed: {ex}");
                return new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                };
            });
            options.Map<BadHttpRequestException>(ex =>
            {
                return new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                };
            });
        });
        return services;
    }
}
