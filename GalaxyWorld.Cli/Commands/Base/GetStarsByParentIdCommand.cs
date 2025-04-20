using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Core.Models;
using System.ComponentModel;
using GalaxyWorld.Cli.Exceptions;
using StarModels = GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class GetStarsByParentIdCommand<TSort> : Command<GetStarsByParentIdCommand<TSort>.Settings>
{
    protected readonly ApiClient _apiClient = new();

    protected abstract string SubPath { get; }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        [Description("ID of the parent entity (e.g., catalogue or constellation)")]
        public int ParentId { get; init; }

        [CommandOption("-s|--sort <sort>")]
        public TSort Sort { get; init; }

        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;

        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;

        [CommandOption("-f|--filter <filter>")]
        public string[]? Filter { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<StarModels::Star>.Parse(filter, null)).ToArray();
            AnsiConsole.MarkupLine($"/{SubPath}/{settings.ParentId}/stars");

            var result = _apiClient.GetWithQueryAsync(
                $"/{SubPath}/{settings.ParentId}/stars",
                (page - 1) * length,
                length,
                settings.Sort,
                filters
            ).Result;

            if (result == null || result.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No stars found for {SubPath} ID {settings.ParentId}[/]");
                return 0;
            }

            AnsiConsole.MarkupLine($"[green]Stars in {SubPath} {settings.ParentId}:[/]");
            foreach (var star in result)
                AnsiConsole.MarkupLine(star.ToString());

            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message ?? "Failed to get stars"}");
            return 1;
        }
    }
}
