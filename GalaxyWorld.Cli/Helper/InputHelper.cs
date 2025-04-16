using Spectre.Console;

namespace GalaxyWorld.Cli.Helper
{
    public static class InputHelper
    {
        public static T Prompt<T>(string label, T defaultValue = default)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<T>($"[green]{label}[/]")
                    .DefaultValue(defaultValue));
        }
    }
}