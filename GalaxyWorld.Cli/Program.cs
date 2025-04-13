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
        ApiStore.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM3ZTA0NDY1NjQ5ZmZhNjA2NTU3NjUwYzdlNjVmMGE4N2FlMDBmZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIxNTc1MTg2NzAyOS1kcnY0dGlscWU1M2RhamozMWl2ZzEwa2xicG4zcTNrMi5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImF1ZCI6IjE1NzUxODY3MDI5LWRydjR0aWxxZTUzZGFqajMxaXZnMTBrbGJwbjNxM2syLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTA3MDE4NTAwMTYwNTQ4MDAyNjE3IiwiZW1haWwiOiJ0ZXN0YnJhbmNoOTlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJvdXdDMnlxaEdaRHAyS1ZtRWcyWmpRIiwiaWF0IjoxNzQ0NTI1NzgzLCJleHAiOjE3NDQ1MjkzODN9.CLjv6IU3kuf_55DlbBAssJyiLkB_4rq6w5tntzuRi1M4YMsLnQzqv22TZJw4t0EtVkaJQU69PXAWXgpafd_-kQ09gCP7wnIEmgIbcoc21h7W1Ksb_-o2fWbfzzmC-eDP6IFwnUAOyq5GAZiCj3u4Qcpqt_bDPh34PBqwjYAaCOJNAGXUjmIJOQ_gvFYisM6qRoXTJDcFSdVqlYMHCuFvUTl6sZV41xiF5fRR9gGidLFcxIOM8o0xA_ppIwJjHUmwumNDAziLu7TBmpns-O5LkjHYQ3RVL9C7j43Os9SI81EMhtmdozgf70v1tTL3h3RNKzjvEGTSLCvAtiRl1bjWIQ";

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
