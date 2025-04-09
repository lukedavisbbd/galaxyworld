using GalaxyWorld.API.Auth;
using GalaxyWorld.API.Database;
using GalaxyWorld.API.Endpoints;
using GalaxyWorld.API.ErrorHandling;
using GalaxyWorld.API.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders()
    .AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvcCore();
builder.Services.ConfigureAuth(builder.Configuration)
    .ConfigureDb(builder.Configuration)
    .ConfigureServices()
    .ConfigureErrorHandling()
    .AddScoped<IMemoryCache, MemoryCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //builder.Configuration.AddJsonFile("")
}

app.UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseProblemDetails();

app.ConfigureEndpoints();

app.Run();
