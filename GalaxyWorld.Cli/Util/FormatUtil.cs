namespace GalaxyWorld.Cli.Util;

public class FormatUtil
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
