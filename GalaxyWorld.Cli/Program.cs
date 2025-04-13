using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.Commands.Catalogue;
using GalaxyWorld.Cli.Commands.Star;
using GalaxyWorld.Cli.Commands.Constellation;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Commands;

class Program
{
    static async Task Main(string[] args)
    {
        ApiStore.BaseUrl = "http://localhost:8080";
        ApiStore.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM3ZTA0NDY1NjQ5ZmZhNjA2NTU3NjUwYzdlNjVmMGE4N2FlMDBmZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIxNTc1MTg2NzAyOS1kcnY0dGlscWU1M2RhamozMWl2ZzEwa2xicG4zcTNrMi5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImF1ZCI6IjE1NzUxODY3MDI5LWRydjR0aWxxZTUzZGFqajMxaXZnMTBrbGJwbjNxM2syLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTA3MDE4NTAwMTYwNTQ4MDAyNjE3IiwiZW1haWwiOiJ0ZXN0YnJhbmNoOTlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJCaEdsazkxb3FqaXc0a3REWW1iVEpRIiwiaWF0IjoxNzQ0NTM5MTU1LCJleHAiOjE3NDQ1NDI3NTV9.stjlZy9h9x4--g8AjDXyKRO45BzlNVpvI6Xjlel3pYV_Zm0dl5ogoim4b0XRM3p9pSHU6EG1gtB3-qfAXlSgiEL57vz3FmECmdM8gTf1COYSro0bMePfirwzpZeTNCEPY4bWboCou1quifAIm74Lvh4eTPcTGpCEMur5cRyCSEZTWXPXilHtybhmKlVKzlvGn6O95pZxRbPonXow7WOuDYIQCUy92a_A8ABUKRc4YdOU72nyf5PRwLK2nYAE2jUoqpjA54Nxa5fg8vxIbwkPD4vnsEFQB5umCxV6U3h2rxe77R5kFh9ep4ChAguX7an_VbaYtJb8qCGybEdQqIl7ig";

        var app = new CommandApp();
        app.Configure(config =>
        {
            // === Catalogue Commands ===
            config.AddCommand<GetAllCataloguesCommand>("get-all-catalogues")
                .WithDescription("List all catalogues");
            config.AddCommand<GetCatalogueByIdCommand>("get-catalogue-by-id")
                .WithDescription("Fetch a specific catalogue by ID");
            config.AddCommand<CreateCatalogueCommand>("create-catalogue")
                .WithDescription("Create a new catalogue");
            config.AddCommand<UpdateCatalogueCommand>("update-catalogue")
                .WithDescription("Update an existing catalogue");
            config.AddCommand<DeleteCatalogueCommand>("delete-catalogue")
                .WithDescription("Delete a catalogue by ID");

            // === Star Commands ===
            config.AddCommand<GetAllStarsCommand>("get-all-stars")
                .WithDescription("List all stars");
            config.AddCommand<GetStarByIdCommand>("get-star-by-id")
                .WithDescription("Fetch a star by ID");
            config.AddCommand<CreateStarCommand>("create-star")
                .WithDescription("Create a new star in a catalogue");
            config.AddCommand<UpdateStarCommand>("update-star")
                .WithDescription("Update a star by ID");
            config.AddCommand<DeleteStarCommand>("delete-star")
                .WithDescription("Delete a star by ID");

            // === Constellation Commands ===
            config.AddCommand<GetAllConstellationsCommand>("get-all-constellations")
                .WithDescription("List all constellations");
            config.AddCommand<GetConstellationByIdCommand>("get-constellation-by-id")
                .WithDescription("Fetch a constellation by ID");
            config.AddCommand<CreateConstellationCommand>("create-constellation")
                .WithDescription("Create a new constellation");
            config.AddCommand<UpdateConstellationCommand>("update-constellation")
                .WithDescription("Update a constellation by ID");
            config.AddCommand<DeleteConstellationCommand>("delete-constellation")
                .WithDescription("Delete a constellation by ID");

            // === Utility Commands ===
            config.AddCommand<HelpCommand>("help")
                .WithDescription("Show help information about commands");
        });

        AnsiConsole.MarkupLine("[bold yellow]GalaxyWorld CLI[/]");
        AnsiConsole.MarkupLine("Type 'help' to view commands.");
        AnsiConsole.MarkupLine("Type 'exit' to quit.\n");

        while (true)
        {
            var input = AnsiConsole.Ask<string>("[blue]>[/]");
            if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                break;

            var splitArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            try
            {
                await app.RunAsync(splitArgs);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Command failed:[/] {ex.Message}");
            }
        }

        AnsiConsole.MarkupLine("[green]Goodbye![/]");
    }
}
