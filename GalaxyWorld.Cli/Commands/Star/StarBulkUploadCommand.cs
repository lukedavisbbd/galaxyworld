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

namespace GalaxyWorld.Cli.Commands.Star;

public class StarBulkUploadCommand : AsyncCommand<StarBulkUploadCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        public required string Path { get; set; }
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
                Console.WriteLine(ex.Message);
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
            var records = csv.GetRecords<StarRecord>();

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
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create star"}[/]");
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
                                    var catSlug = catalogues.First(c => c.CatId == entry.CatId).CatSlug;
                                    return $"{catSlug}: '{entry.EntryId}'";
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
                AnsiConsole.MarkupLine($"[red]upload failed[/]");

                totalFailures += BATCH_SIZE;

                throw;
            }

            i++;
        }

        return (totalSuccesses, totalFailures);
    }

    static async Task<IEnumerable<(StarModels::StarBulkResponse, List<EntryModels::CatalogueEntryInsertWithStar>)>> UploadBatch(ApiClient client, IEnumerable<StarRecord> starRecords, List<ConModels::Constellation> constellations, List<CatModels::Catalogue> catalogues)
    {
        var athygCatId = catalogues.First(c => c.CatSlug == "athyg").CatId;
        var tycho2CatId = catalogues.First(c => c.CatSlug == "tycho2").CatId;
        var gaiaDr3CatId = catalogues.First(c => c.CatSlug == "gaia3").CatId;
        var hygV3CatId = catalogues.First(c => c.CatSlug == "hyg").CatId;
        var hipparcosCatId = catalogues.First(c => c.CatSlug == "hipparcos").CatId;
        var henryDraperCatId = catalogues.First(c => c.CatSlug == "hd").CatId;
        var harvardYaleCatId = catalogues.First(c => c.CatSlug == "ybs").CatId;
        var glieseCatId = catalogues.First(c => c.CatSlug == "gliese").CatId;
        var bayerCatId = catalogues.First(c => c.CatSlug == "bayer").CatId;
        var flamsteedCatId = catalogues.First(c => c.CatSlug == "flamsteed").CatId;

        var allCatEntries = new List<List<EntryModels::CatalogueEntryInsertWithStar>>();

        var starInserts = starRecords.Select(record =>
        {
            record.Constellation = NormaliseNullableString(record.Constellation);

            ConModels::Constellation? constellation =
                record.Constellation != null
                ?
                constellations.First(con => con.IauAbbr.Equals(record.Constellation, StringComparison.OrdinalIgnoreCase))
                :
                null;

            record.ProperName = NormaliseNullableString(record.ProperName);
            record.PosSrc = NormaliseNullableString(record.PosSrc) ?? throw new ArgumentNullException();
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

            string? flamsteedDesignation = null;
            if (record.FlamsteedDesignation != null && constellation != null)
                flamsteedDesignation = $"{record.FlamsteedDesignation} {constellation.Genitive}";

            var catEntries = new List<EntryModels::CatalogueEntryInsertWithStar>();
            if (record.AthygId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = athygCatId,
                    EntryId = record.AthygId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.Tycho2Id))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = tycho2CatId,
                    EntryId = record.Tycho2Id,
                };
                catEntries.Add(catEntry);
            }
            if (record.GaiaDr3Id.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = gaiaDr3CatId,
                    EntryDesignation = record.GaiaDr3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HygV3Id.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = hygV3CatId,
                    EntryId = record.HygV3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HipparcosId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = hipparcosCatId,
                    EntryId = record.HipparcosId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HenryDraperId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = henryDraperCatId,
                    EntryDesignation = record.HenryDraperId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HarvardYaleId.HasValue)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = harvardYaleCatId,
                    EntryDesignation = record.HarvardYaleId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.GlieseId))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = glieseCatId,
                    EntryId = record.GlieseId,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.BayerDesignation))
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = bayerCatId,
                    EntryDesignation = record.BayerDesignation,
                };
                catEntries.Add(catEntry);
            }
            if (flamsteedDesignation != null)
            {
                var catEntry = new EntryModels::CatalogueEntryInsertWithStar
                {
                    CatId = flamsteedCatId,
                    EntryDesignation = flamsteedDesignation,
                };
                catEntries.Add(catEntry);
            }

            allCatEntries.Add(catEntries);

            var starInsert = new StarModels::StarInsert
            {
                Constellation = constellation?.ConId,
                ProperName = record.ProperName,
                RightAscension = record.RightAscension,
                Declination = record.Declination,
                PosSrc = record.PosSrc,
                Distance = record.Distance,
                X0 = record.X0,
                Y0 = record.Y0,
                Z0 = record.Z0,
                DistanceSrc = record.DistanceSrc,
                Magnitude = record.Magnitude,
                AbsoluteMagnitude = record.AbsoluteMagnitude,
                ColourIndex = record.ColourIndex,
                MagnitudeSrc = record.MagnitudeSrc,
                RadialVelocity = record.RadialVelocity,
                RadialVelocitySrc = record.RadialVelocitySrc,
                ProperMotionRightAscension = record.ProperMotionRightAscension,
                ProperMotionDeclination = record.ProperMotionDeclination,
                ProperMotionSrc = record.ProperMotionSrc,
                VelocityX = record.VelocityX,
                VelocityY = record.VelocityY,
                VelocityZ = record.VelocityZ,
                SpectralType = record.SpectralType,
                SpectralTypeSrc = record.SpectralTypeSrc,
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