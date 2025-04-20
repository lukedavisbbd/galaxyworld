using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using System.ComponentModel;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class DeleteEntityByIdCommand<T> : Command<DeleteEntityByIdCommand<T>.Settings> where T : class
{
    protected readonly ApiClient _apiClient = new();
    protected abstract string Path { get; }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        [Description("The ID of the entity to delete")]
        public int Id { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var result = _apiClient.DeleteAsync<T>($"/{Path}/{settings.Id}").Result;
            AnsiConsole.MarkupLine($"[green]{typeof(T).Name} with ID {settings.Id} deleted successfully.[/]");
            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message ?? "Failed to delete "+typeof(T).Name}");
            return 1;
        }
    }
}
