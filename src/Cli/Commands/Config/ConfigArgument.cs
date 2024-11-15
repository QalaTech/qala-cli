using System.ComponentModel;
using Spectre.Console.Cli;

namespace Cli.Commands.Config;

public class ConfigArgument : CommandSettings
{
    [CommandOption("-k|--api-key <API_KEY>")]
    [Description("The API key to use for the request.")]
    public string Key { get; set; } = string.Empty;
    // [CommandOption("-e|--environment-id <ENVIRONMENT_ID>")]
    // public string EnvironmentId { get; set; } = string.Empty;
}
