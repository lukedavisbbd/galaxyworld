# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./GalaxyWorld.Cli/GalaxyWorld.Cli.csproj"
RUN dotnet publish "./GalaxyWorld.Cli/GalaxyWorld.Cli.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GalaxyWorld.Cli.dll"]