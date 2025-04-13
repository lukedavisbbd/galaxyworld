using Spectre.Console.Cli;

namespace GalaxyWorld.Cli.Commands.Shared
{
    public abstract class ApiCommandBase<TSettings> : AsyncCommand<TSettings>
        where TSettings : CommandSettings
    {
        protected string ApiUrl => "https://localhost:8080"; // Default API base
    }
}