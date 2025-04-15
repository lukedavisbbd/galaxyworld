using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.Commands.Catalogue;
using GalaxyWorld.Cli.Commands.Star;
using GalaxyWorld.Cli.Commands.Constellation;
using GalaxyWorld.Cli.Commands.Auth;
using GalaxyWorld.Cli.Commands.EntriesCommands;
using GalaxyWorld.Cli.ApiHandler;
using System.Text.Json;

namespace GalaxyWorld.Cli;

public static class Program
{
    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static void WelcomeMessage()
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("GalaxyWorld")
                .Centered()
                .Color(Color.MediumPurple));

        AnsiConsole.Write(
            new Panel("[bold white]Explore the stars. Navigate the cosmos.[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(new Style(Color.Blue))
                .Padding(1, 1, 1, 1)
                .Header("[bold yellow]Welcome to the GalaxyWorld CLI[/]", Justify.Center)
                .Collapse());

        AnsiConsole.MarkupLine("[grey]Type [bold]--help[/], [bold]login[/], [bold]stars get-all[/], or [bold]exit[/] to begin...[/]");
        AnsiConsole.WriteLine();
    }

    public static async Task Main(string[] args)
    {
        var token = await LoginCommand.LoginFromFile();
        ApiClient.DefaultAuthToken = token;

        WelcomeMessage();

        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddBranch("stars", config =>
            {
                config.AddCommand<GetAllStarsCommand>("get-all").WithDescription("List all stars");
                config.AddCommand<GetStarByIdCommand>("get").WithDescription("Fetch a star by ID");
                config.AddCommand<CreateStarCommand>("create").WithDescription("Create a new star");
                config.AddCommand<UpdateStarCommand>("update").WithDescription("Update a star by ID");
                config.AddCommand<DeleteStarCommand>("delete").WithDescription("Delete a star by ID");
            });

            config.AddBranch("catalogues", config =>
            {
                config.AddCommand<GetAllCataloguesCommand>("get-all").WithDescription("List all catalogues");
                config.AddCommand<GetCatalogueByIdCommand>("get").WithDescription("Fetch a specific catalogue by ID");
                config.AddCommand<CreateCatalogueCommand>("create").WithDescription("Create a new catalogue");
                config.AddCommand<UpdateCatalogueCommand>("update").WithDescription("Update an existing catalogue");
                config.AddCommand<DeleteCatalogueCommand>("delete").WithDescription("Delete a catalogue by ID");
            });

            config.AddBranch("constellations", config =>
            {
                config.AddCommand<GetAllConstellationsCommand>("get-all").WithDescription("List all constellations");
                config.AddCommand<GetConstellationByIdCommand>("get").WithDescription("Fetch a constellation by ID");
                config.AddCommand<DrawConstellationCommand>("draw").WithDescription("Draw a constellation by ID");
                config.AddCommand<CreateConstellationCommand>("create").WithDescription("Create a new constellation");
                config.AddCommand<UpdateConstellationCommand>("update").WithDescription("Update a constellation by ID");
                config.AddCommand<DeleteConstellationCommand>("delete").WithDescription("Delete a constellation by ID");
            });

            config.AddBranch("entries", config =>
            {
                config.AddCommand<GetStarCatalogueEntriesCommand>("get-star-entries").WithDescription("Get all catalogue entries for a star");
                config.AddCommand<GetCatalogueStarEntriesCommand>("get-catalogue-entries").WithDescription("Get all star entries for a catalogue");
                config.AddCommand<GetStarCatalogueEntryCommand>("get-star-entry").WithDescription("Get a specific entry for a star from a catalogue");
                config.AddCommand<GetCatalogueStarEntryCommand>("get-catalogue-entry").WithDescription("Get a specific entry from a catalogue for a star");
                config.AddCommand<CreateStarCatalogueEntryCommand>("post-star-entry").WithDescription("Create an entry linking a star to a catalogue");
                config.AddCommand<CreateCatalogueStarEntryCommand>("post-catalogue-entry").WithDescription("Create an entry linking a catalogue to a star");
                config.AddCommand<UpdateStarCatalogueEntryCommand>("patch-star-entry").WithDescription("Update notes for a catalogue entry linked to a star");
                config.AddCommand<UpdateCatalogueStarEntryCommand>("patch-catalogue-entry").WithDescription("Update notes for a catalogue entry linked to a catalogue");
                config.AddCommand<DeleteStarCatalogueEntryCommand>("delete-star-entry").WithDescription("Delete a star's entry in a catalogue");
                config.AddCommand<DeleteCatalogueStarEntryCommand>("delete-catalogue-entry").WithDescription("Delete a catalogue's entry for a star");
            });

            config.AddCommand<LoginCommand>("login").WithDescription("Login with Google.");
            config.AddCommand<LogoutCommand>("logout").WithDescription("End current session.");
        });

        while (true)
        {
            AnsiConsole.Markup("[bold blue]>[/] ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Trim().ToLower() == "exit")
                break;

            var commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            try
            {
                await app.RunAsync(commandArgs);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Command failed:[/] {ex.Message}");
            }
        }
    }
}