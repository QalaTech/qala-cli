using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Config;

public class ConfigArgument : CommandSettings
{
    [CommandOption("-k|--key <API_KEY>")]
    [Description("The API key to use for the request.")]
    public string Key { get; set; } = string.Empty;
    [CommandOption("-e|--env <ENVIRONMENT_ID>")]
    [Description("The environment ID to use for the request.")]
    public string EnvironmentId { get; set; } = string.Empty;
}
