using Spectre.Console;
using Spectre.Console.Cli;

namespace GalaxyWorld.Cli.Commands
{
    public class HelpCommand : Command<HelpCommand.Settings>
    {
        public class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupLine("[bold underline yellow]GalaxyWorld CLI - Help[/]");
            AnsiConsole.WriteLine();

            var table = new Table()
                .Title("Available Commands")
                .AddColumn("Command")
                .AddColumn("Description");

            // Catalogue Commands
            table.AddRow("get-all-catalogues", "List all catalogues");
            table.AddRow("get-catalogue-by-id <id>", "Fetch a specific catalogue by ID");
            table.AddRow("create-catalogue -n <name> -d <description>", "Create a new catalogue");
            table.AddRow("update-catalogue <id> -n <name> -d <description>", "Update a catalogue");
            table.AddRow("delete-catalogue <id>", "Delete a catalogue");

            // Star Commands
            table.AddRow("get-all-stars", "List all stars");
            table.AddRow("get-star-by-id <id>", "Fetch a star by ID");
            table.AddRow("create-star -n <name> -c <catalogueId>", "Create a new star");
            table.AddRow("update-star <id> -n <name> -c <catalogueId>", "Update a star");
            table.AddRow("delete-star <id>", "Delete a star");

            // Constellation Commands
            table.AddRow("get-all-constellations", "List all constellations");
            table.AddRow("get-constellation-by-id <id>", "Fetch a constellation by ID");
            table.AddRow("create-constellation -n <name>", "Create a new constellation");
            table.AddRow("update-constellation <id> -n <name>", "Update a constellation");
            table.AddRow("delete-constellation <id>", "Delete a constellation");

            AnsiConsole.Write(table);
            return 0;
        }
    }
}
