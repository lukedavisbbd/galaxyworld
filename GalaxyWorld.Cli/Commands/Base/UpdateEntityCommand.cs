using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Exceptions;
using System.ComponentModel;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class UpdateEntityCommand<TPatch, TResponse> : AsyncCommand<UpdateEntityCommand<TPatch, TResponse>.Settings>
    where TPatch : class
    where TResponse : class
{
    protected readonly ApiClient _apiClient = new();

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        [Description("ID of the entity to update")]
        public int Id { get; set; }
    }

    protected abstract string Path { get; }

    protected abstract TPatch BuildPatch();

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var patch = BuildPatch();
            var result = await _apiClient.PatchAsync<TResponse, TPatch>($"/{Path}/{settings.Id}", patch);

            AnsiConsole.MarkupLine($"[bold green]Updated {typeof(TResponse).Name}:[/]");
            ModelHelper.PrintModel(result);
            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message ?? "Failed to update "+typeof(TResponse).Name}[/]");
            return 1;
        }
    }
}
