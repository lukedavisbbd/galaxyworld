using Spectre.Console;
using Spectre.Console.Cli;

namespace GalaxyWorld.Cli.Commands.Auth;

public class LogoutCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configPath = Path.Join(appdataPath, "GalaxyWorld");
        var idTokenPath = Path.Join(configPath, "id_token");

        try
        {
            if (!File.Exists(idTokenPath))
                throw new FileNotFoundException();

            File.Delete(idTokenPath);
            AnsiConsole.MarkupLine("[green]Logout successful.[/]");
            return 0;
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine("[green]No session.[/]");
            return 1;
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine("[red]Failed to delete.[/]");
            return 1;
        }
    }
}
