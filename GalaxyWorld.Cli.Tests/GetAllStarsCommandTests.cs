using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Commands.Star;

public class GetAllStarsCommandTests
{
    private class FakeApiClient
    {
        public Func<int, int, StarSort, Filter<Star>[], Task<List<Star>>>? OnGetStars;

        public Task<List<Star>> GetStars(int start, int length, StarSort sort, Filter<Star>[]? filters)
        {
            return OnGetStars?.Invoke(start, length, sort, filters) ?? Task.FromResult(new List<Star>());
        }
    }

    private class GetAllStarsCommandShim : AsyncCommand<GetAllStarsCommand.Settings>
    {
        private readonly FakeApiClient _client;
        private readonly IAnsiConsole _console;

        public GetAllStarsCommandShim(FakeApiClient client, IAnsiConsole console)
        {
            _client = client;
            _console = console;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, GetAllStarsCommand.Settings settings)
        {
            try
            {
                var page = int.Max(settings.Page, 1);
                var length = int.Max(settings.Length, 1);
                var filters = (settings.Filter ?? []).Select(f => Filter<Star>.Parse(f, null)).ToArray();

                var stars = await _client.GetStars((page - 1) * length, length, settings.Sort, filters);

                if (stars == null || stars.Count == 0)
                {
                    _console.MarkupLine("[yellow]No stars found.[/]");
                    return 0;
                }

                var table = new Table().Title($"[bold]Stars[/] (Sorted by {settings.Sort})").AddColumns(
                    "Star ID", "Constellation ID", "Proper Name", "Distance", "Magnitude", "Spectral Type");

                foreach (var star in stars)
                {
                    table.AddRow(
                        star.StarId.ToString(),
                        star.ConstellationId?.ToString() ?? "",
                        star.ProperName ?? "",
                        star.Distance?.ToString() ?? "",
                        star.Magnitude.ToString(),
                        star.SpectralType ?? "");
                }

                _console.Write(table);
                _console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
                return 0;
            }
            catch (AppException e)
            {
                _console.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get stars list."}[/]");
                return 1;
            }
        }
    }

    [Fact]
    public async Task GetAllStarsCommand_ShouldOutputNoStarsFound_WhenApiReturnsEmptyList()
    {
        // Arrange
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeApi = new FakeApiClient
        {
            OnGetStars = (_, _, _, _) => Task.FromResult(new List<Star>())
        };

        var command = new GetAllStarsCommandShim(fakeApi, testConsole);
        var settings = new GetAllStarsCommand.Settings
        {
            Page = 1,
            Length = 10,
            Sort = StarSort.ProperName,
            Filter = []
        };

        // Act
        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        // Assert
        Assert.Contains("No stars found", output);
        Assert.Equal(0, result);
    }
}
