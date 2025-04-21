using Spectre.Console;

namespace GalaxyWorld.Cli.Helper;

public static class TableHelper
{
    public static void Print<T>(
        IEnumerable<T> items,
        string title,
        Dictionary<string, Func<T, string>> columns)
    {
        if (items == null || !items.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No data to display.[/]");
            return;
        }

        var table = new Table().Title($"[bold green]{title}[/]");

        foreach (var column in columns.Keys)
        {
            table.AddColumn(column);
        }

        foreach (var item in items)
        {
            var row = columns.Values.Select(selector => selector(item) ?? "").ToArray();
            table.AddRow(row);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}
