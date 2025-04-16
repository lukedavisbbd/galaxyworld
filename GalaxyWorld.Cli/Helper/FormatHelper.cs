namespace GalaxyWorld.Cli.Helper;

public class FormatHelper
{
    public static string PascalToTitleCase(string pascalCase)
    {
        var outResult = "";

        foreach (var c in pascalCase)
        {
            if (char.IsUpper(c))
                outResult += " ";
            outResult += c;
        }

        return outResult.TrimStart();
    }
}
