using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class CreateEntityCommand<TModel, TResponse> : Command where TModel : class where TResponse : class
{
    protected readonly ApiClient _apiClient = new();

    protected abstract string Path { get; }

    protected abstract TModel BuildModel();

    public override int Execute(CommandContext context)
    {
        try
        {
            var model = BuildModel();
            var result = _apiClient.PostAsync<TResponse, TModel>($"/{Path}", model).Result;

            AnsiConsole.MarkupLine($"[green]Created {typeof(TResponse).Name}:[/]");
            ModelHelper.PrintModel(result);
            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message ?? "Failed to create "+typeof(TResponse).Name}");
            return 1;
        }
    }
}
