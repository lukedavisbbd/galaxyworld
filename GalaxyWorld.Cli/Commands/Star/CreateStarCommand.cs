using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.CatalogueEntry;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using StarModels = GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class CreateStarCommand :  AsyncCommand<CreateStarCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            public string Name { get; set; } = "";

            [CommandOption("-c|--catalogueId <CATALOGUE_ID>")]
            public int CatalogueId { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();

            var entries = new List<CatalogueEntryInsertWithStar>();
            while (AnsiConsole.Confirm("Add a catalogue entry?"))
            {
                entries.Add(new CatalogueEntryInsertWithStar
                {
                    CatId = InputHelper.PromptInt("Catalogue Entry - Cat ID"),
                    EntryId = InputHelper.PromptString("Catalogue Entry - Entry ID"),
                    EntryDesignation = InputHelper.PromptString("Catalogue Entry - Designation")
                });
            }

            var payload = new StarModels::StarInsert
            {
                Declination = InputHelper.Prompt<double>("Declination"),
                Magnitude = InputHelper.Prompt<double>("Magnitude"),
                MagnitudeSrc = InputHelper.Prompt<string>("Magnitude Source"),
                PosSrc = InputHelper.Prompt<string>("Position Source"),
                RightAscension = InputHelper.Prompt<double>("Right Ascension"),
                Constellation = InputHelper.Prompt<int>("Constellation"),
                ProperName = InputHelper.Prompt<string>("ProperName"),
                Distance = InputHelper.Prompt<double>("Distance"),
                X0 = InputHelper.Prompt<double>("X0"),
                Y0 = InputHelper.Prompt<double>("Y0"),
                Z0 = InputHelper.Prompt<double>("Z0"),
                DistanceSrc = InputHelper.Prompt<string>("Distance Source"),
                AbsoluteMagnitude = InputHelper.Prompt<double>("Absolute Magnitude"),
                ColourIndex = InputHelper.Prompt<double>("Colour Index"),
                RadialVelocity = InputHelper.Prompt<double>("Radial Velocity"),
                RadialVelocitySrc = InputHelper.Prompt<string>("Radial Velocity Source"),
                ProperMotionRightAscension = InputHelper.Prompt<double>("Proper Motion RA"),
                ProperMotionDeclination = InputHelper.Prompt<double>("Proper Motion Dec"),
                ProperMotionSrc = InputHelper.Prompt<string>("Proper Motion Source"),
                VelocityX = InputHelper.Prompt<double>("Velocity X"),
                VelocityY = InputHelper.Prompt<double>("Velocity Y"),
                VelocityZ = InputHelper.Prompt<double>("Velocity Z"),
                SpectralType = InputHelper.Prompt<string>("Spectral Type"),
                SpectralTypeSrc = InputHelper.Prompt<string>("Spectral Type Source"),
                CatalogueEntries = entries
            };

            try {
                var star = await client.PostStar(payload);

                if (star is null)
                {
                    AnsiConsole.MarkupLine("[yellow]No star created.[/]");
                    return 0;
                }

                var table = new Table().Title("[bold]Star Created[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(StarModels::Star).GetProperties())
                {
                    var value = prop.GetValue(star)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create star"}[/]");
                return 1;
            }
        }
    }
}