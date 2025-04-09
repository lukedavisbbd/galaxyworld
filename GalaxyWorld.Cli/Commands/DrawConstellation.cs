using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli.Commands;

internal class DrawConstellation
{
    static string Grey(byte greyness)
    {
        return $"\x1b[38;2;{greyness};{greyness};{greyness}m";
    }

    static string Reset()
    {
        return $"\x1b[0m";
    }

    public static void DrawStars(List<Star> stars)
    {
        var raMin = decimal.MaxValue;
        var raMax = decimal.MinValue;
        var decMin = decimal.MaxValue;
        var decMax = decimal.MinValue;

        foreach (var star in stars)
        {
            if (star.RightAscension < raMin)
                raMin = star.RightAscension;
            if (star.RightAscension > raMax)
                raMax = star.RightAscension;
            if (star.Declination < decMin)
                decMin = star.Declination;
            if (star.Declination > decMax)
                decMax = star.Declination;
        }

        var raDelta = raMax - raMin;
        var decDelta = decMax - decMin;

        var width = Console.WindowWidth;
        var height = Console.WindowHeight;

        var newRaDelta = (decimal)width / height * decDelta / 16;
        raMin -= (newRaDelta - raDelta) / 2;
        raMax += (newRaDelta - raDelta) / 2;
        raDelta = newRaDelta;

        //// add 10% space on each side
        raMin -= raDelta * 0.1m;
        raMax += raDelta * 0.1m;
        decMin -= decDelta * 0.1m;
        decMax += decDelta * 0.1m;
        raDelta = raMax - raMin;
        decDelta = decMax - decMin;

        Console.WriteLine($"W: {width}, H: {height}");
        Console.WriteLine($"RA_min: {raMin}, RA_max: {raMax}, RA_delta: {raDelta}");
        Console.WriteLine($"DEC_min: {decMin}, DEC_max: {decMax}, DEC_delta: {decDelta}");

        var chars = new string[width, height];

        for (int y = height - 1; y >= 0; y--)
            for (int x = 0; x < width; x++)
                chars[x, y] = " ";

        stars.Sort((a, b) => b.Magnitude.CompareTo(a.Magnitude));

        foreach (var star in stars)
        {
            var x = (int)decimal.Round((star.RightAscension - raMin) / raDelta * width);
            var y = (int)decimal.Round((star.Declination - decMin) / decDelta * height);
            x = int.Clamp(x, 0, width - 1);
            y = int.Clamp(y, 0, height - 1);

            const string CHARSET = "X*.";

            //const double BRIGHTNESS_MAX = 1.0e-1;
            //const double BRIGHTNESS_MIN = 1.0e-10;
            
            //var brightness = double.Pow(2.5, -(double)star.Magnitude);

            const double BRIGHTNESS_MAX = 0.0;
            const double BRIGHTNESS_MIN = 8.0;

            var brightness = (double)star.Magnitude;

            var lerp = (brightness - BRIGHTNESS_MIN) / (BRIGHTNESS_MAX - BRIGHTNESS_MIN);
            var i = (int)((1.0 - lerp) * CHARSET.Length);
            var greyness = (byte)int.Clamp((int)(lerp * 196 + 59), 0, 255);
            
            //Console.WriteLine($"{brightness}, {i}, {lerp}, {lerp * CHARSET.Length}");
            
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
    }

    public static char ToBrailleChar(bool[,] array)
    {
        var bits = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                int n = y < 4 ? y + 3 * x : x + 6;
                bits |= array[x, y] ? 1 << n : 0;
            }
        }

        return (char)(bits + 0x2800);
    }
}
