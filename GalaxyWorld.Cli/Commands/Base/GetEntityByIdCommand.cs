using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class GetEntityByIdCommand<T> : Command<GetEntityByIdCommand<T>.Settings> where T : class
{
    protected readonly ApiClient _apiClient = new();

    protected abstract string Path { get; }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        [Description($"The ID of the entity to get")]
        public int Id { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var result = _apiClient.GetAsync<T>($"/{Path}/{settings.Id}").Result;
            if (result != null)
            {
                AnsiConsole.MarkupLine($"[bold green]{typeof(T).Name} Found:[/]");
                ModelHelper.PrintModel(result);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]{typeof(T).Name} not found with ID {settings.Id}[/]");
            }

            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message ?? "Failed to get "+typeof(T).Name}[/]");
            return 1;
        }
    }
}
