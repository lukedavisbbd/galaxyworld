using Spectre.Console;

namespace GalaxyWorld.Cli.Helper;

public static class CliWelcome
{
    public static void Show()
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
}
