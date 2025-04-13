using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;
using System.ComponentModel;
using GalaxyWorld.Cli.Exceptions;
using ConstellationModel = GalaxyWorld.Core.Models.Constellation;
using StarModel = GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class DrawConstellationCommand : AsyncCommand<DrawConstellationCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<constellationId>")]
            public int ConstellationId { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {

            var client = new ApiClient();

            try 
            {
                var constellation = await client.GetConstellation(settings.ConstellationId);
                var stars = await client.GetConstellationStars(settings.ConstellationId);
                DrawStars(constellation, stars);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to draw constellation."}[/]");
                return 1;
            }
        }

        static string Grey(byte greyness) => $"[38;2;{greyness};{greyness};{greyness}m";
        static string Yellow() => "[0;33m";
        static string Reset() => "[0m";
        static string MoveUp(int n) => $"[{n}A";
        static string MoveDown(int n) => $"[{n}B";

        public static void DrawStars(ConstellationModel::Constellation constellation, List<StarModel::Star> stars)
        {
            var raMin = double.MaxValue;
            var raMax = double.MinValue;
            var decMin = double.MaxValue;
            var decMax = double.MinValue;

            foreach (var star in stars)
            {
                if (star.RightAscension < raMin) raMin = star.RightAscension;
                if (star.RightAscension > raMax) raMax = star.RightAscension;
                if (star.Declination < decMin) decMin = star.Declination;
                if (star.Declination > decMax) decMax = star.Declination;
            }

            var raDelta = raMax - raMin;
            var decDelta = decMax - decMin;

            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            var newRaDelta = (double)width / height * decDelta / 16;
            raMin -= (newRaDelta - raDelta) / 2;
            raMax += (newRaDelta - raDelta) / 2;
            raDelta = newRaDelta;

            raMin -= raDelta * 0.1;
            raMax += raDelta * 0.1;
            decMin -= decDelta * 0.1;
            decMax += decDelta * 0.1;
            raDelta = raMax - raMin;
            decDelta = decMax - decMin;

            var chars = new string[width, height];
            for (int y = height - 1; y >= 0; y--)
                for (int x = 0; x < width; x++)
                    chars[x, y] = " ";

            stars.Sort((a, b) => b.Magnitude.CompareTo(a.Magnitude));

            foreach (var star in stars)
            {
                var x = (int)double.Round((star.RightAscension - raMin) / raDelta * width);
                var y = (int)double.Round((star.Declination - decMin) / decDelta * height);
                if (x < 0 || y < 0 || x >= width || y >= height) continue;

                const string CHARSET = "X*.";
                const double BRIGHTNESS_MAX = 0.0;
                const double BRIGHTNESS_MIN = 8.0;

                var brightness = (double)star.Magnitude;
                var lerp = (brightness - BRIGHTNESS_MIN) / (BRIGHTNESS_MAX - BRIGHTNESS_MIN);
                var i = (int)((1.0 - lerp) * CHARSET.Length);
                var greyness = (byte)int.Clamp((int)(lerp * 196 + (255 - 196)), 0, 255);

                i = int.Clamp(i, 0, CHARSET.Length - 1);
                if (CHARSET[i] != ' ')
                {
                    chars[x, y] = $"{Grey(greyness)}{CHARSET[i]}{Reset()}";
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Console.Write(chars[x, y]);
                Console.WriteLine();
            }
            Console.Write(MoveUp(height));

            var infoWidth = int.Clamp(width / 3, 24, width);
            var info = WrapString($"Name: {constellation.ConName}", infoWidth) + '\n'
                     + WrapString($"IAU Abbr.: {constellation.IauAbbr}", infoWidth) + '\n'
                     + WrapString($"NASA Abbr.: {constellation.NasaAbbr}", infoWidth) + '\n'
                     + WrapString($"Genitive: {constellation.Genitive}", infoWidth) + '\n'
                     + WrapString($"Origin: {constellation.Origin}", infoWidth) + '\n'
                     + WrapString($"Meaning: {constellation.Meaning}", infoWidth);

            var newLines = info.Count(c => c == '\n');
            Console.WriteLine($"{Yellow()}{info}{Reset()}");
            Console.Write(MoveDown(height - newLines));
        }

        public static string WrapString(string str, int width)
        {
            for (int i = (str.Length - 1) / width; i > 0; i--)
                str = str.Insert(i * width, "\n");
            return str;
        }
    }
}