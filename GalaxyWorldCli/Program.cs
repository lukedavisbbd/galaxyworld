using System.Globalization;
using System.Net.Http.Json;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;

namespace GalaxyWorldCli;

class Program
{
    private class Options
    {
        [Option("uploadCsv", HelpText = "Upload CSV file of containing stars in the ATHYG database format.", Required = true)]
        public required string? UploadCsvPath { get; init; }
    }

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async options =>
            {
                if (options.UploadCsvPath != null)
                {
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
                            if (Star.HeaderMappings.ContainsKey(args.Header))
                                return Star.HeaderMappings[args.Header];

                            if (Star.HeaderMappings.ContainsValue(args.Header))
                                return args.Header;

                            throw new KeyNotFoundException(args.Header);
                        },
                    };
                    
                    var csv = new CsvReader(reader, config);
                    var records = csv.GetRecords<Star>();

                    AddStars(records).Wait();
                }
            });
    }

    static async Task AddStars(IEnumerable<Star> stars)
    {
        var total = stars.Count();

        var tasks = new List<Task>();

        var i = 0;
        foreach (var star in stars)
        {
            Console.WriteLine($"creating star {i}...");
            tasks.Add(CreateStar(star, i, total));
            i++;
        }

        await Task.WhenAll(tasks);
    }

    private static HttpClient client = new HttpClient();
    static async Task CreateStar(Star star, int i, int total)
    {
        var status = "failed";
        try
        {
            var resp = await client.PostAsJsonAsync("https://localhost:4433/stars", star);
            status = "success";
        }
        finally
        {
            Console.WriteLine($"{i}/{total} - {status}");
        }
    }
}