using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.Commands.Catalogue;
using GalaxyWorld.Cli.Commands.Star;
using GalaxyWorld.Cli.Commands.Constellation;
using GalaxyWorld.Cli.ApiHandler;
using System.Text.Json;
using GalaxyWorld.Cli.Commands.Auth;

namespace GalaxyWorld.Cli;

class Program
{
    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static async Task Main(string[] args)
    {
        var token = await LoginCommand.LoginFromFile();
        ApiClient.DefaultAuthToken = token;

        // if trying to do anything but view the help menu
        if (args.Length > 0 && args[0].ToLower() != "--help" && args[0].ToLower() != "-h")
        {
            // trying to login, but already logged in
            if (args[0] == "login" && token != null)
            {
                AnsiConsole.MarkupLine($"[green]Logged in.[/]");
                return;
            }

            // trying to do something without having logged in
            if (args[0] != "login" && token == null)
            {
                AnsiConsole.MarkupLine($"[red]Not logged in. Login with the 'login' command.[/]");
                return;
            }
        }

        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddBranch("stars", config =>
            {
                // === Star Commands ===
                config.AddCommand<GetAllStarsCommand>("get-all")
                    .WithDescription("List all stars");
                config.AddCommand<GetStarByIdCommand>("get")
                    .WithDescription("Fetch a star by ID");
                config.AddCommand<CreateStarCommand>("create")
                    .WithDescription("Create a new star in a catalogue");
                config.AddCommand<UpdateStarCommand>("update")
                    .WithDescription("Update a star by ID");
                config.AddCommand<DeleteStarCommand>("delete")
                    .WithDescription("Delete a star by ID");
            });

            config.AddBranch("catalogues", config =>
            {
                // === Catalogue Commands ===
                config.AddCommand<GetAllCataloguesCommand>("get-all")
                    .WithDescription("List all catalogues");
                config.AddCommand<GetCatalogueByIdCommand>("get")
                    .WithDescription("Fetch a specific catalogue by ID");
                config.AddCommand<CreateCatalogueCommand>("create")
                    .WithDescription("Create a new catalogue");
                config.AddCommand<UpdateCatalogueCommand>("update")
                    .WithDescription("Update an existing catalogue");
                config.AddCommand<DeleteCatalogueCommand>("delete")
                    .WithDescription("Delete a catalogue by ID");
            });

            config.AddBranch("constellations", config =>
            {
                // === Constellation Commands ===
                config.AddCommand<GetAllConstellationsCommand>("get-all")
                    .WithDescription("List all constellations");
                config.AddCommand<GetConstellationByIdCommand>("get")
                    .WithDescription("Fetch a constellation by ID");
                config.AddCommand<DrawConstellationCommand>("draw")
                    .WithDescription("Drawa constellation by ID");
                config.AddCommand<CreateConstellationCommand>("create")
                    .WithDescription("Create a new constellation");
                config.AddCommand<UpdateConstellationCommand>("update")
                    .WithDescription("Update a constellation by ID");
                config.AddCommand<DeleteConstellationCommand>("delete")
                    .WithDescription("Delete a constellation by ID");
            });

            config.AddCommand<LogoutCommand>("logout")
                .WithDescription("End current session.");

            config.AddCommand<LoginCommand>("login")
                .WithDescription("Login with Google.");
        });

        try
        {
            await app.RunAsync(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Command failed:[/] {ex.Message}");
        }
    }
}
