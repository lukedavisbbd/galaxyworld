using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Commands.Auth;
using Microsoft.Extensions.DependencyInjection;
using GalaxyWorld.Cli.Commands.Setup;
using GalaxyWorld.Cli.Helper;

public static class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            CliWelcome.Show();

            var token = await LoginCommand.LoginFromFile();
            ApiClient.DefaultAuthToken = token;

            if (token == null)
            {
                AnsiConsole.MarkupLine($"[red]Not logged in. Login with the 'login' command.[/]");
            }

            var services = new ServiceCollection();
            services.AddSingleton<ApiClient>();

            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);

            app.Configure(CommandConfigurator.Configure);

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
