
using System.Text.RegularExpressions;

public static class CommandLineStringSplitter
{
    public static IEnumerable<string> Split(string input)
    {
        var matches = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+");
        foreach (Match match in matches)
            yield return match.Value.Trim('"');
    }
}
