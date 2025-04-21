using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Commands.Auth;
using GalaxyWorld.Cli.Commands.Stars;
using GalaxyWorld.Cli.Commands.Catalogues;
using GalaxyWorld.Cli.Commands.Constellations;
using GalaxyWorld.Cli.Commands.CatalogueEntries;
using Microsoft.Extensions.DependencyInjection;

public static class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            AnsiConsole.MarkupLine("[bold blue]Welcome to GalaxyWorld CLI![/]");
            AnsiConsole.MarkupLine("[italic gray]Type 'exit' to leave the shell[/]\n");

            var token = await LoginCommand.LoginFromFile();
            ApiClient.DefaultAuthToken = token;

            if (token == null)
            {
                AnsiConsole.MarkupLine($"[red]Not logged in. Login with the 'login' command.[/]");
            }

            // if (args.Length > 0 && args[0].ToLower() != "--help" && args[0].ToLower() != "-h")
            // {
            //     if (args[0] == "login" && token != null)
            //     {
            //         AnsiConsole.MarkupLine($"[green]Logged in.[/]");
            //         return;
            //     }

            //     if (args[0] != "login" && token == null)
            //     {
            //         AnsiConsole.MarkupLine($"[red]Not logged in. Login with the 'login' command.[/]");
            //         return;
            //     }
            // }

            var services = new ServiceCollection();
            services.AddSingleton<ApiClient>();

            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);

            app.Configure(config =>
            {
                config.AddBranch("stars", config =>
                {
                    // === Star Commands ===
                    config.AddCommand<GetAllStarsCommand>("get-all")
                        .WithDescription("List all stars");
                    config.AddCommand<GetStarByIdCommand>("get")
                        .WithDescription("Fetch a star by ID");
                    config.AddCommand<GetStarPlanetsCommand>("planets")
                        .WithDescription("Fetch star planetary system information");
                    config.AddCommand<CreateStarCommand>("create")
                        .WithDescription("Register a star");
                    config.AddCommand<StarBulkUploadCommand>("upload-bulk")
                        .WithDescription("Upload a ATHYG format CSV file of stars");
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
                    config.AddCommand<GetCatalogueStarsCommand>("stars")
                        .WithDescription("List all star entries");
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
                    config.AddCommand<GetConstellationStarsCommand>("stars")
                        .WithDescription("Fetch all stars in a constellation");
                    config.AddCommand<DrawConstellationCommand>("draw")
                        .WithDescription("Draw a constellation by ID");
                    config.AddCommand<CreateConstellationCommand>("create")
                        .WithDescription("Create a new constellation");
                    config.AddCommand<UpdateConstellationCommand>("update")
                        .WithDescription("Update a constellation by ID");
                    config.AddCommand<DeleteConstellationCommand>("delete")
                        .WithDescription("Delete a constellation by ID");
                });

                config.AddBranch("entries", config =>
                {
                    config.AddCommand<GetCatalogueEntryCommand>("get")
                        .WithDescription("Get a specific entry for catalogue ID and star ID");
                    config.AddCommand<CreateCatalogueEntryCommand>("create")
                        .WithDescription("Create an entry linking a star to a catalogue");
                    config.AddCommand<UpdateCatalogueEntryCommand>("update")
                        .WithDescription("Update entry ID or designation for a catalogue entry");
                    config.AddCommand<DeleteCatalogueEntryCommand>("delete")
                        .WithDescription("Delete a star's entry in a catalogue");
                });

                config.AddCommand<LogoutCommand>("logout")
                    .WithDescription("End current session.");
                config.AddCommand<LoginCommand>("login")
                    .WithDescription("Login with Google.");
                config.ValidateExamples();
            });

            while (true)
            {
                AnsiConsole.Markup("[bold blue]> [/] ");
                var input = Console.ReadLine();
                AnsiConsole.WriteLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine("[blue]Goodbye![/]");
                    break;
                }
                else if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                try
                {
                    var splitArgs = CommandLineStringSplitter.Split(input).ToArray();
                    await app.RunAsync(splitArgs);

                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Command failed:[/] {ex.Message}");
        }
    }
}
