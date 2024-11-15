using Spectre.Console.Cli;

namespace Cli.Commands.Config;

public class ConfigArgument : CommandSettings
{
    [CommandOption("-k|--api-key <API_KEY>")]
    public string Key { get; set; } = string.Empty;
    [CommandOption("-e|--environment-id <ENVIRONMENT_ID>")]
    public string EnvironmentId { get; set; } = string.Empty;
}
