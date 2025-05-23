﻿using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli.Helper;

public class DrawConstellation
{
    static string Grey(byte greyness)
    {
        return $"\x1b[38;2;{greyness};{greyness};{greyness}m";
    }

    static string Yellow()
    {
        return "\x1b[0;33m";
    }

    static string Reset()
    {
        return $"\x1b[0m";
    }

    static string MoveUp(int n)
    {
        return $"\x1b[{n}A";
    }

    static string MoveDown(int n)
    {
        return $"\x1b[{n}B";
    }

    public static void DrawStars(Constellation constellation, List<Star> stars)
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
        var height = Console.WindowHeight - 1;

        var newRaDelta = (decimal)width / height * decDelta / 16;
        raMin -= (newRaDelta - raDelta) / 2;
        raMax += (newRaDelta - raDelta) / 2;
        raDelta = newRaDelta;

        // add 10% space on each side
        raMin -= raDelta * 0.1m;
        raMax += raDelta * 0.1m;
        decMin -= decDelta * 0.1m;
        decMax += decDelta * 0.1m;
        raDelta = raMax - raMin;
        decDelta = decMax - decMin;

        var chars = new string[width, height];

        for (int y = height - 1; y >= 0; y--)
            for (int x = 0; x < width; x++)
                chars[x, y] = " ";

        stars.Sort((a, b) => b.Magnitude.CompareTo(a.Magnitude));

        foreach (var star in stars)
        {
            var x = (int)decimal.Round((star.RightAscension - raMin) / raDelta * width);
            var y = (int)decimal.Round((star.Declination - decMin) / decDelta * height);
            if (x < 0 || y < 0 || x >= width || y >= height) continue;

            const string CHARSET = "X*.";

            // max is lower than min because a higher magnitude star is dimmer
            const decimal BRIGHTNESS_MAX = 0.0m;
            const decimal BRIGHTNESS_MIN = 8.0m;

            var brightness = (decimal)star.Magnitude;

            var lerp = (brightness - BRIGHTNESS_MIN) / (BRIGHTNESS_MAX - BRIGHTNESS_MIN);
            var i = (int)((1.0m - lerp) * CHARSET.Length);
            var greyness = (byte)int.Clamp((int)(lerp * 255), 0, 255);

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

        var info = WrapString($"Name: {constellation.ConstellationName}", infoWidth) + '\n'
            + WrapString($"IAU Abbr.: {constellation.IauAbbreviation}", infoWidth) + '\n'
            + WrapString($"NASA Abbr.: {constellation.NasaAbbreviation}", infoWidth) + '\n'
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
