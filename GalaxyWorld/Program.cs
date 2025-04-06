using GalaxyWorld.Auth;
using GalaxyWorld.Database;
using GalaxyWorld.Endpoints;
using GalaxyWorld.ErrorHandling;
using GalaxyWorld.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

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
