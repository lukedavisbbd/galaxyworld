using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using GalaxyWorld.Cli.Commands;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli;

class Program
{
    public const string BASE_URL = "https://localhost:4433";
    public const int BATCH_SIZE = 500;

    private static HttpClient client = new HttpClient();

    private class Options
    {
        [Option("uploadCsv", HelpText = "Upload CSV file of containing stars in the ATHYG database format.")]
        public required string? UploadCsvPath { get; init; }
        [Option("drawConstellation", HelpText = "Draw.")]
        public required string? DrawConstellationPath { get; init; }
    }

    public const string ID_TOKEN = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM3ZTA0NDY1NjQ5ZmZhNjA2NTU3NjUwYzdlNjVmMGE4N2FlMDBmZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIyMzg1ODk0ODg4MjYtcm01MG50djN0N2Q2a2k1MzlkYzdlZTJqamY2azViaGsuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIyMzg1ODk0ODg4MjYtcm01MG50djN0N2Q2a2k1MzlkYzdlZTJqamY2azViaGsuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTIyNDk2MjE3MDQ0MjgzMTQ3OTIiLCJlbWFpbCI6Imx1a2UuZGF2aXNAYmJkLmNvLnphIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJkMTZqRG8tdVVJWUdPczFXZmt2WUtnIiwibmFtZSI6Ikx1a2UgRGF2aXMiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jS3VhaEdvQXo3RlNVYlpwcmhwSFV2NUlQMldTSmRjWlYyWWR1eEg1dWJnN2VZcXB3PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6Ikx1a2UiLCJmYW1pbHlfbmFtZSI6IkRhdmlzIiwiaWF0IjoxNzQ0Mjg5NDgyLCJleHAiOjE3NDQyOTMwODJ9.r82Y1dhxSqCaYjwkb2Z1CsVPScroxL8-ZoRQDI7_phGeXSHE7IG9QYInVfo-O2sR-kpPuwlq9h5LDp2jk5wrwKH2vGtUFBoLbgvc6okr89cuETXZzqF3N7ZOdEr8sQUq1IsF489ZBFU5OL0drWlV5bSjTW_DHP1QntJUqmDg8QBr8AJrEMzGqyll895bZco0_ceIQDVa76DQQZhTawvEWPISM72geOUik8F_upO00VJak77PaSLq9XacevQSyrHzY5KEdX13NZ0j38ivYjcnCISzf0slHd1zUVaNcNlBHuWzPtImkovtbwCwIN4nhcuPOJmoxYT1_fR3cqI3e9UPkQ";

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                if (options.UploadCsvPath != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ID_TOKEN);

                    StreamReader reader;
                    
                    try
                    {
                        reader = new StreamReader(options.UploadCsvPath);
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

                    AddStars(records).Wait();
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

    static async Task AddStars(IEnumerable<StarRecord> starRecords)
    {
        var constellations = await client.GetFromJsonAsync<List<Constellation>>($"{BASE_URL}/constellations")
            ?? throw new ArgumentNullException();
        var catalogues = await client.GetFromJsonAsync<List<Catalogue>>($"{BASE_URL}/catalogues")
            ?? throw new ArgumentNullException();

        var tasks = new List<Task>();

        var batches = starRecords.Chunk(BATCH_SIZE);

        var i = 0;
        foreach (var batch in batches)
        {
            try
            {
                await UploadBatch(batch, constellations, catalogues);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Batch error: {ex}");
                throw;
            }
            Console.WriteLine($"uploaded batch {i}");
            i++;
        }
    }

    static async Task UploadBatch(IEnumerable<StarRecord> starRecords, List<Constellation> constellations, List<Catalogue> catalogues)
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
                    EntryId = record.GaiaDr3Id.ToString()!,
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
        var stars = await response.Content.ReadFromJsonAsync<List<Star>>()
            ?? throw new ArgumentNullException();

        // AthygId athyg
        // Tycho2Id tycho2
        // GaiaDr3Id gaia3
        // HygV3Id hyg
        // HipparcosId hipparcos
        // HenryDraperId hd
        // HarvardYaleId ybs
        // GlieseId gliese
        // BayerId bayer
        // FlamsteedId flamsteed
    }

    static string? NormaliseNullableString(string? str)
    {
        str = str?.Trim();
        if (string.IsNullOrWhiteSpace(str))
            str = null;
        return str;
    }
}