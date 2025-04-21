using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class CreateEntityCommand<TModel, TResponse> : AsyncCommand where TModel : class where TResponse : class
{
    protected readonly ApiClient _apiClient = new();

    protected abstract string Path { get; }

    protected abstract TModel BuildModel();

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            var model = BuildModel();
            var result = await _apiClient.PostAsync<TResponse, TModel>($"/{Path}", model);

            AnsiConsole.MarkupLine($"[bold green]Created {typeof(TResponse).Name}:[/]");
            ModelHelper.PrintModel(result);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create "+typeof(TResponse).Name}[/]");
            return 1;
        }
    }
}
