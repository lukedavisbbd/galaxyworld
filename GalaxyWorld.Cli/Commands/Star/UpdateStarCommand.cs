using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using StarModel = GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Helper;
using StarModels = GalaxyWorld.Core.Models.Star;
using CoreModels = GalaxyWorld.Core.Models;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class UpdateStarCommand : AsyncCommand<UpdateStarCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }

            [CommandOption("-n|--name <NAME>")]
            public string? Name { get; set; }

            [CommandOption("-c|--catalogueId <CATALOGUE_ID>")]
            public int? CatalogueId { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();

            try 
            {
                var payload = new StarModels::StarPatch
                {
                    Declination = new CoreModels::Optional<double>(InputHelper.Prompt<double>("Declination")),
                    Magnitude = new CoreModels::Optional<double>(InputHelper.Prompt<double>("Magnitude")),
                    MagnitudeSrc = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Magnitude Source")),
                    PosSrc = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Position Source")),
                    RightAscension = new CoreModels::Optional<double>(InputHelper.Prompt<double>("Right Ascension")),
                    Constellation = new CoreModels::Optional<int?>(InputHelper.Prompt<int>("Constellation")),
                    ProperName = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("ProperName")),
                    Distance = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Distance")),
                    X0 = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("X0")),
                    Y0 = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Y0")),
                    Z0 = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Z0")),
                    DistanceSrc = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("Distance Source")),
                    AbsoluteMagnitude = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Absolute Magnitude")),
                    ColourIndex = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Colour Index")),
                    RadialVelocity = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Radial Velocity")),
                    RadialVelocitySrc = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("Radial Velocity Source")),
                    ProperMotionRightAscension = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Proper Motion RA")),
                    ProperMotionDeclination = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Proper Motion Dec")),
                    ProperMotionSrc = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("Proper Motion Source")),
                    VelocityX = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Velocity X")),
                    VelocityY = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Velocity Y")),
                    VelocityZ = new CoreModels::Optional<double?>(InputHelper.Prompt<double>("Velocity Z")),
                    SpectralType = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("Spectral Type")),
                    SpectralTypeSrc = new CoreModels::Optional<string?>(InputHelper.Prompt<string>("Spectral Type Source")),
                };

                var star = await client.PatchStar(settings.Id, payload);

                var table = new Table().Title("[bold]Star Updated[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(StarModel::Star).GetProperties())
                {
                    var value = prop.GetValue(star)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e) 
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to update star."}[/]");
                return 1;
            }
        }
    }
}