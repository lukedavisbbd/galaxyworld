using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using GalaxyWorldCli.Models;

namespace GalaxyWorldCli;

class Program
{
    public const string BASE_URL = "https://localhost:4433";
    public const int BATCH_SIZE = 500;

    private static HttpClient client = new HttpClient();

    private class Options
    {
        [Option("uploadCsv", HelpText = "Upload CSV file of containing stars in the ATHYG database format.", Required = true)]
        public required string? UploadCsvPath { get; init; }
    }

    public const string ID_TOKEN = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM3ZTA0NDY1NjQ5ZmZhNjA2NTU3NjUwYzdlNjVmMGE4N2FlMDBmZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIyMzg1ODk0ODg4MjYtcm01MG50djN0N2Q2a2k1MzlkYzdlZTJqamY2azViaGsuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIyMzg1ODk0ODg4MjYtcm01MG50djN0N2Q2a2k1MzlkYzdlZTJqamY2azViaGsuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTIyNDk2MjE3MDQ0MjgzMTQ3OTIiLCJlbWFpbCI6Imx1a2UuZGF2aXNAYmJkLmNvLnphIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJ3V0FaSzlialAxMlc1OHNKY1Y1WEd3IiwibmFtZSI6Ikx1a2UgRGF2aXMiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jS3VhaEdvQXo3RlNVYlpwcmhwSFV2NUlQMldTSmRjWlYyWWR1eEg1dWJnN2VZcXB3PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6Ikx1a2UiLCJmYW1pbHlfbmFtZSI6IkRhdmlzIiwiaWF0IjoxNzQ0MTI3ODA4LCJleHAiOjE3NDQxMzE0MDh9.iQn_Fv80WfCA4pIKaXH_BjV3I3XB5Ui1vVMo1Mv8jVyXU4u5c-f9v1ehnmnFkCVtteSX41jqVWVgg1NuebtZIAUoKU4vMGF5uNhjZRciTPtSPsqFBRxiTLz2HyJB22pzbBu-gLgsLhBnwLY-tA9f73SqTBQzVTagtCdAUzrWujBxwDiATPIW2LzrRWhjScDXFs6MRGNzi8W43p9waZNHB7ZrM4BcWCyhimwFQw7Zn4-xlxpjGxtXMtatu54jVj0lq0Ygi2twzk6nloY04Q6WJLnnaNVgP1WXULSXXMlvx7DcmntRqjt4XxBJ7CEgRROvXEVIlvqhiAnT7O8USJTlcg";

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
        var parsedRecords = starRecords.Select(record =>
        {
            record.Constellation = NormaliseNullableString(record.Constellation);

            int? constellation = 
                record.Constellation != null
                ?
                constellations.First(con => con.IauAbbr.Equals(record.Constellation, StringComparison.OrdinalIgnoreCase)).ConId
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
            record.BayerId = NormaliseNullableString(record.BayerId);

            var starIds = new StarIds
            {
                AthygId = record.AthygId,
                Tycho2Id = record.Tycho2Id,
                GaiaDr3Id = record.GaiaDr3Id,
                HygV3Id = record.HygV3Id,
                HipparcosId = record.HipparcosId,
                HenryDraperId = record.HenryDraperId,
                HarvardYaleId = record.HarvardYaleId,
                GlieseId = record.GlieseId,
                BayerId = record.BayerId,
                FlamsteedId = record.FlamsteedId,
            };

            var starInsert = new StarInsert
            {
                Constellation = constellation,
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
            };

            return (starIds, starInsert);
        }).ToList();

        var starIds = parsedRecords.Select(record => record.starIds);
        var inserts = parsedRecords.Select(record => record.starInsert);

        var response = await client.PostAsJsonAsync($"{BASE_URL}/stars/bulk", inserts);
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

        foreach (var (star, ids) in stars.Zip(starIds))
        {
            if (ids.AthygId.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = athygCatId,
                    EntryId = ids.AthygId.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (!string.IsNullOrWhiteSpace(ids.Tycho2Id)) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = tycho2CatId,
                    EntryId = ids.Tycho2Id,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.GaiaDr3Id.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = gaiaDr3CatId,
                    EntryId = ids.GaiaDr3Id.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.HygV3Id.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = hygV3CatId,
                    EntryId = ids.HygV3Id.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.HipparcosId.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = hipparcosCatId,
                    EntryId = ids.HipparcosId.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.HenryDraperId.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = henryDraperCatId,
                    EntryId = ids.HenryDraperId.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.HarvardYaleId.HasValue) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = harvardYaleCatId,
                    EntryId = ids.HarvardYaleId.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (!string.IsNullOrWhiteSpace(ids.GlieseId)) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = glieseCatId,
                    EntryId = ids.GlieseId,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (!string.IsNullOrWhiteSpace(ids.BayerId)) {
                var catEntry = new CatalogueStarEntryStar {
                    CatId = bayerCatId,
                    EntryId = ids.BayerId,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
            if (ids.FlamsteedId.HasValue) {
                var catEntry = new CatalogueStarEntryStar
                {
                    CatId = flamsteedCatId,
                    EntryId = ids.FlamsteedId.ToString()!,
                };
                await client.PostAsJsonAsync($"{BASE_URL}/stars/{star.StarId}/catentries", catEntry);
            }
        }
    }

    static string? NormaliseNullableString(string? str)
    {
        str = str?.Trim();
        if (string.IsNullOrWhiteSpace(str))
            str = null;
        return str;
    }
}