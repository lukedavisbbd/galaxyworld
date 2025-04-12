using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using GalaxyWorld.Cli.Commands;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli;

class Program
{
    public const string BASE_URL = "https://localhost:4433";
    public const int BATCH_SIZE = 100;

    private static HttpClient client = new HttpClient();
    public static JsonSerializerOptions JsonOptions { get {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }

    private class Options
    {
        [Option("uploadAthygCsv", HelpText = "Upload CSV file of containing stars in the ATHYG database format.")]
        public required string? UploadAthygCsvPath { get; init; }
        [Option("skipFailures", HelpText = "For bulk operations, do not print or take action on failures.")]
        public required bool SkipFailures { get; init; } = false;
        [Option("drawConstellation", HelpText = "Draw constellation.")]
        public required string? DrawConstellationPath { get; init; }
    }

    public static void Main(string[] args)
    {
        AuthService.LoginOAuth2();

        Console.WriteLine(AuthService.AuthToken);

        return;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                if (options.UploadAthygCsvPath != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.AuthToken);

                    StreamReader reader;
                    
                    try
                    {
                        reader = new StreamReader(options.UploadAthygCsvPath);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
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

                    AddStars(records, options.SkipFailures).Wait();
                }
                if (options.DrawConstellationPath != null)
                {
                    var starsFile = File.ReadAllText(options.DrawConstellationPath);
                    var stars = JsonSerializer.Deserialize<List<Star>>(starsFile, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    }) ?? throw new ArgumentNullException();
                    DrawConstellation.DrawStars(new Constellation
                    {
                        ConId = 30,
                        ConName = "Crux",
                        IauAbbr = "Cru",
                        NasaAbbr = "Cruc",
                        Genitive = "Crucis",
                        Origin = "1589, Plancius, split from Centaurus",
                        Meaning = "southern cross",
                    }, stars);
                }
            });
    }

    static async Task AddStars(IEnumerable<StarRecord> starRecords, bool skipFailures)
    {
        var constellations = await client.GetFromJsonAsync<List<Constellation>>($"{BASE_URL}/constellations")
            ?? throw new ArgumentNullException();
        var catalogues = await client.GetFromJsonAsync<List<Catalogue>>($"{BASE_URL}/catalogues")
            ?? throw new ArgumentNullException();

        var tasks = new List<Task>();

        var batches = starRecords.Chunk(BATCH_SIZE);

        var i = 0;

        var totalFailures = 0;

        foreach (var batch in batches)
        {
            Console.WriteLine($"Uploading star batch #{i} of {batch.Length} stars");
            try
            {
                var stars = await UploadBatch(batch, constellations, catalogues);
                
                var successes = stars.Where(s => s.Status == Status.Success).Count();
                var failures = stars.Where(s => s.Status == Status.Failure).Count();
                var failureReasons = stars.Select((star, index) => (star, index)).Where((s) => s.star.Status == Status.Failure)
                    .Select(s => $"#{s.index + i * BATCH_SIZE}: {s.star.Error}");
                
                totalFailures += failures;

                Console.Write($"Added {successes} stars");

                if (failures > 0 && !skipFailures)
                    Console.Write($", failed to add {failures} stars\n{string.Join("\n", failureReasons)}");

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"All {BATCH_SIZE} stars failed, upload error: {ex}");
                
                totalFailures += BATCH_SIZE;
                
                throw;
            }

            i++;
        }
    }

    static async Task<IEnumerable<StarBulkResponse>> UploadBatch(IEnumerable<StarRecord> starRecords, List<Constellation> constellations, List<Catalogue> catalogues)
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

        var starInserts = starRecords.Select(record =>
        {
            record.Constellation = NormaliseNullableString(record.Constellation);

            Constellation? constellation = 
                record.Constellation != null
                ?
                constellations.First(con => con.IauAbbr.Equals(record.Constellation, StringComparison.OrdinalIgnoreCase))
                :
                null;
            
            record.PosSrc = NormaliseNullableString(record.PosSrc) ?? throw new ArgumentNullException();
            record.DistanceSrc = NormaliseNullableString(record.DistanceSrc);
            record.MagnitudeSrc = NormaliseNullableString(record.MagnitudeSrc) ?? throw new ArgumentNullException();
            record.RadialVelocitySrc = NormaliseNullableString(record.RadialVelocitySrc);
            record.ProperMotionSrc = NormaliseNullableString(record.ProperMotionSrc);
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

            var catEntries = new List<CatalogueEntryInsertWithStar>();
            if (record.AthygId.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = athygCatId,
                    EntryId = record.AthygId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.Tycho2Id))
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = tycho2CatId,
                    EntryId = record.Tycho2Id,
                };
                catEntries.Add(catEntry);
            }
            if (record.GaiaDr3Id.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = gaiaDr3CatId,
                    EntryDesignation = record.GaiaDr3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HygV3Id.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = hygV3CatId,
                    EntryId = record.HygV3Id.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HipparcosId.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = hipparcosCatId,
                    EntryId = record.HipparcosId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HenryDraperId.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = henryDraperCatId,
                    EntryId = record.HenryDraperId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (record.HarvardYaleId.HasValue)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = harvardYaleCatId,
                    EntryId = record.HarvardYaleId.ToString()!,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.GlieseId))
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = glieseCatId,
                    EntryId = record.GlieseId,
                };
                catEntries.Add(catEntry);
            }
            if (!string.IsNullOrWhiteSpace(record.BayerDesignation))
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = bayerCatId,
                    EntryDesignation = record.BayerDesignation,
                };
                catEntries.Add(catEntry);
            }
            if (flamsteedDesignation != null)
            {
                var catEntry = new CatalogueEntryInsertWithStar
                {
                    CatId = flamsteedCatId,
                    EntryDesignation = flamsteedDesignation,
                };
                catEntries.Add(catEntry);
            }

            var starInsert = new StarInsert
            {
                Constellation = constellation?.ConId,
                ProperName = string.IsNullOrWhiteSpace(record.ProperName) ? null : record.ProperName.Trim(),
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

        var response = await client.PostAsJsonAsync($"{BASE_URL}/stars/bulk", starInserts);
        var stars = await response.Content.ReadFromJsonAsync<List<StarBulkResponse>>(JsonOptions)
            ?? throw new ArgumentNullException();

        return stars;
    }

    static string? NormaliseNullableString(string? str)
    {
        str = str?.Trim();
        if (string.IsNullOrWhiteSpace(str))
            str = null;
        return str;
    }
}