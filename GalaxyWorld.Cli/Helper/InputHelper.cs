using Spectre.Console;

namespace GalaxyWorld.Cli.Helper
{
    public static class InputHelper
    {
        public static string PromptString(string label, string defaultValue = "")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[green]{label}[/]")
                    .DefaultValue(defaultValue)
                    .AllowEmpty());
        }

        public static int PromptInt(string label, int defaultValue = 0)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>($"[green]{label}[/]")
                    .DefaultValue(defaultValue));
        }
    }
}