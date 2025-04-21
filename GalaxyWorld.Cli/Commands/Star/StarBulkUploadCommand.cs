using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using EntryModels = GalaxyWorld.Core.Models.CatalogueEntry;
using CoreModels = GalaxyWorld.Core.Models;
using StarModels = GalaxyWorld.Core.Models.Star;
using ConModels = GalaxyWorld.Core.Models.Constellation;
using CatModels = GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Exceptions;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Spectre.Console;
using GalaxyWorld.Cli.Models;
using System.Text.Json;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.Cli.Commands.Star;

public class StarBulkUploadCommand : AsyncCommand<StarBulkUploadCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        public required string Path { get; set; }
        [CommandOption("-s|--skip <skip_n_rows>")]
        public int SkipNRows { get; set; } = 0;
    }

    const int BATCH_SIZE = 100;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {

            StreamReader reader;

            try
            {
                reader = new StreamReader(settings.Path);
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                return 1;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args =>
                {
                    if (StarRecord.HeaderMappings.ContainsKey(args.Header))
                        return StarRecord.HeaderMappings[args.Header];

                    if (StarRecord.HeaderMappings.ContainsValue(args.Header))
                        return args.Header;

                    throw new KeyNotFoundException(args.Header);
                },
            };

            var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<StarRecord>()
                .Where((r, i) => i >= settings.SkipNRows);

            try
            {
                var (successes, failures) = await UploadStars(client, records);

                Console.WriteLine($"Uploaded {successes} stars, failed to upload {failures} stars.");

                return 0;
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions[0];
            }
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to create star"}[/]");
            return 1;
        }
    }

    static async Task<(int, int)> UploadStars(ApiClient client, IEnumerable<StarRecord> starRecords)
    {
        var constellations = await client.GetConstellations();
        var catalogues = await client.GetCatalogues();

        var tasks = new List<Task>();

        var batches = starRecords.Chunk(BATCH_SIZE);

        var i = 0;

        var totalSuccesses = 0;
        var totalFailures = 0;

        foreach (var batch in batches)
        {
            Console.Write($"Uploading star batch #{i} of {batch.Length} stars... ");
            try
            {
                var stars = await UploadBatch(client, batch, constellations, catalogues);

                var successes = stars.Where(s => s.Item1.Status == CoreModels::Status.Success).Count();
                var failures = stars.Where(s => s.Item1.Status == CoreModels::Status.Failure).Count();
                var failureReasons = stars.Select((resp, index) => (resp, index)).Where((s) => s.resp.Item1.Status == CoreModels::Status.Failure)
                    .Select(s =>
                    {
                        var catalogueInfo = "";
                        if (s.resp.Item1.Error!.Contains("Entry ID already taken")) {
                            var entries = s.resp.Item2
                                .Where(entry => entry.EntryId != null)
                                .Select(entry =>
                                {
                                    var catalogueSlug = catalogues.First(c => c.CatalogueId == entry.CatalogueId).CatalogueSlug;
                                    return $"{catalogueSlug}: '{entry.EntryId}'";
                                });
                            catalogueInfo = ' ' + string.Join(", ", entries);
                        }
                        return $"#{s.index + i * BATCH_SIZE}: {s.resp.Item1.Error}{catalogueInfo}";
                    });

                totalSuccesses += successes;
                totalFailures += failures;

                Console.Write($"added {successes} stars");

                if (failures > 0)
                    Console.Write($", failed to add {failures} stars\n{string.Join("\n", failureReasons)}");

                Console.WriteLine();
            }
            catch (Exception)
            {
                AnsiConsole.MarkupLine("[red]upload failed[/]");

                totalFailures += BATCH_SIZE;

                throw;
            }

            i++;
        }

        return (totalSuccesses, totalFailures);
    }

    static async Task<IEnumerable<(StarModels::StarBulkResponse, List<EntryModels::CatalogueEntryInsertWithStar>)>> UploadBatch(ApiClient client, IEnumerable<StarRecord> starRecords, List<ConModels::Constellation> constellations, List<CatModels::Catalogue> catalogues)
    {
        var athygCatalogueId = catalogues.First(c => c.CatalogueSlug == "athyg").CatalogueId;
        var tycho2CatalogueId = catalogues.First(c => c.CatalogueSlug == "tycho2").CatalogueId;
        var gaiaDr3CatalogueId = catalogues.First(c => c.CatalogueSlug == "gaia3").CatalogueId;
        var hygV3CatalogueId = catalogues.First(c => c.CatalogueSlug == "hyg").CatalogueId;
        var hipCatalogueId = catalogues.First(c => c.CatalogueSlug == "hip").CatalogueId;
        var henryDraperCatalogueId = catalogues.First(c => c.CatalogueSlug == "hd").CatalogueId;
        var harvardYaleCatalogueId = catalogues.First(c => c.CatalogueSlug == "ybs").CatalogueId;
        var glieseCatalogueId = catalogues.First(c => c.CatalogueSlug == "gliese").CatalogueId;
        var bayerCatalogueId = catalogues.First(c => c.CatalogueSlug == "bayer").CatalogueId;
        var flamCatalogueId = catalogues.First(c => c.CatalogueSlug == "flam").CatalogueId;

        var allCatEntries = new List<List<EntryModels::CatalogueEntryInsertWithStar>>();

        var starInserts = starRecords.Select(record =>
        {
            record.Constellation = NormaliseNullableString(record.Constellation);

            ConModels::Constellation? constellation =
                record.Constellation != null
                ?
                constellations.First(con => con.IauAbbreviation.Equals(record.Constellation, StringComparison.OrdinalIgnoreCase))
                :
                null;

            record.ProperName = NormaliseNullableString(record.ProperName);
            record.DistanceSrc = NormaliseNullableString(record.DistanceSrc);
            record.MagnitudeSrc = NormaliseNullableString(record.MagnitudeSrc) ?? throw new ArgumentNullException();
            record.RadialVelocitySrc = NormaliseNullableString(record.RadialVelocitySrc);
            record.ProperMotionSrc = NormaliseNullableString(record.ProperMotionSrc);
            record.SpectralType = NormaliseNullableString(record.SpectralType);
            record.SpectralTypeSrc = NormaliseNullableString(record.SpectralTypeSrc);

            record.Tycho2Id = NormaliseNullableString(record.Tycho2Id);
            record.GlieseId = NormaliseNullableString(record.GlieseId);
            record.BayerDesignation = NormaliseNullableString(record.BayerDesignation);

            var constellationGenetive = constellation;

            if (record.BayerDesignation != null && constellation != null)
                record.BayerDesignation = $"{record.BayerDesignation} {constellation.Genitive}";
            else
                record.BayerDesignation = null;

            string? flamDesignation = null;
            if (record.FlamsteedDesignation != null && constellation != null)
                flamDesignation = $"{record.FlamsteedDesignation} {constellation.Genitive}";

            var catEntries = new List<EntryModels::CatalogueEntryInsertWithStar>();
            if (record.AthygId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = athygCatalogueId,
                    EntryId = record.AthygId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.Tycho2Id))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = tycho2CatalogueId,
                    EntryId = record.Tycho2Id,
                };
                catEntries.Add(catEntry);
            }
            if (record.GaiaDr3Id.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = gaiaDr3CatalogueId,
                    EntryDesignation = record.GaiaDr3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HygV3Id.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = hygV3CatalogueId,
                    EntryId = record.HygV3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HipparcosId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = hipCatalogueId,
                    EntryId = record.HipparcosId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HenryDraperId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = henryDraperCatalogueId,
                    EntryDesignation = record.HenryDraperId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HarvardYaleId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = harvardYaleCatalogueId,
                    EntryDesignation = record.HarvardYaleId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.GlieseId))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = glieseCatalogueId,
                    EntryId = record.GlieseId,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.BayerDesignation))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = bayerCatalogueId,
                    EntryDesignation = record.BayerDesignation,
                };
                catEntries.Add(catEntry);
            }
            if (flamDesignation != null)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatalogueId = flamCatalogueId,
                    EntryDesignation = flamDesignation,
                };
                catEntries.Add(catEntry);
            }

            allCatEntries.Add(catEntries);

            Vector? positionCartesian = null;
            if (record.X0.HasValue && record.Y0.HasValue && record.Z0.HasValue)
            {
                positionCartesian = new Vector
                {
                    X = record.X0.Value,
                    Y = record.Y0.Value,
                    Z = record.Z0.Value,
                };
            }

            Vector? velocityCartesian = null;
            if (record.VelocityX.HasValue && record.VelocityY.HasValue && record.VelocityZ.HasValue)
            {
                velocityCartesian = new Vector
                {
                    X = record.VelocityX.Value,
                    Y = record.VelocityY.Value,
                    Z = record.VelocityZ.Value,
                };
            }

            Vector? velocityCircular = null;
            if (record.ProperMotionRightAscension.HasValue && record.ProperMotionDeclination.HasValue && record.RadialVelocity.HasValue)
            {
                velocityCircular = new Vector
                {
                    X = record.ProperMotionRightAscension.Value,
                    Y = record.ProperMotionDeclination.Value,
                    Z = record.RadialVelocity.Value,
                };
            }

            var starInsert = new StarModels::StarInsert
            {
                ConstellationId = constellation?.ConstellationId,
                ProperName = record.ProperName,
                RightAscension = record.RightAscension,
                Declination = record.Declination,
                Distance = record.Distance,
                PositionCartesian = positionCartesian,
                VelocityCartesian = velocityCartesian,
                SpectralType = record.SpectralType,
                Magnitude = record.Magnitude,
                ColourIndex = record.ColourIndex,
                VelocityCircular = velocityCircular,
                CatalogueEntries = catEntries,
            };

            return starInsert;
        }).ToList();

        var stars = await client.PostStarsBulk(starInserts);

        return stars.Zip(allCatEntries);
    }

    static string? NormaliseNullableString(string? str)
    {
        str = str?.Trim();
        if (string.IsNullOrWhiteSpace(str))
            str = null;
        return str;
    }
}