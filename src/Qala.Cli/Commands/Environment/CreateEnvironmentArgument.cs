using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class CreateEnvironmentArgument : CommandSettings
{
    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the environment.")]
    public string Name { get; set; } = string.Empty;
    [CommandOption("-r|--region <REGION>")]
    [Description("The region of the environment.")]
    public string Region { get; set; } = string.Empty;
    [CommandOption("-t|--type <TYPE>")]
    [Description("The type of the environment.")]
    public string Type { get; set; } = string.Empty;
}