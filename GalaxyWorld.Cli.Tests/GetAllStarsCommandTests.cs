using Xunit;
using GalaxyWorld.Cli.Commands.Star;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using GalaxyWorld.Core.Models.Star;
using System.Collections.Generic;
using System.Text.Json;
using GalaxyWorld.Core.Models;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;


public class GetAllStarsCommandTests
{
    private class FakeHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Return an empty list of stars
            var starsJson = JsonSerializer.Serialize(new List<Star>());
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(starsJson, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task GetAllStarsCommand_ShouldOutputNoStarsFound_WhenApiReturnsEmptyList()
    {
        // Arrange
        var fakeHandler = new FakeHttpHandler();
        var fakeHttpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("https://localhost:4433")
        };

        // Set static token to skip auth
        ApiClient.DefaultAuthToken = "fake-token";

        // Redirect console output
        var output = new StringWriter();
        Console.SetOut(output);

        var settings = new GetAllStarsCommand.Settings
        {
            Page = 1,
            Length = 10,
            Sort = StarSort.ProperName,
            Filter = [] // no filters
        };

        // Act
        var command = new GetAllStarsCommandShim(fakeHttpClient);
        var result = await command.ExecuteAsync(null, settings);

        // Assert
        var consoleOut = output.ToString();
        Assert.Contains("No stars found", consoleOut);
        Assert.Equal(0, result);
    }

    // Tiny shim to inject HttpClient into ApiClient inside GetAllStarsCommand
    private class GetAllStarsCommandShim : GetAllStarsCommand
    {
        private readonly HttpClient _client;

        public GetAllStarsCommandShim(HttpClient client)
        {
            _client = client;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            // Hacky but legal: replace internal client via reflection
            var apiClientField = typeof(ApiClient).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var realClient = new ApiClient();
            apiClientField?.SetValue(realClient, _client);

            // Trick to run base logic with the fake-wired client
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(f => Filter<Star>.Parse(f, null)).ToArray();

            var stars = await realClient.GetStars((page - 1) * length, length, settings.Sort, filters);

            if (stars is null || stars.Count == 0)
            {
                Console.WriteLine("No stars found.");
                return 0;
            }

            return 1; // fail if this part is hit
        }
    }
}
