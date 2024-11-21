using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class SetEnvironmentArgument : CommandSettings
{
    [CommandOption("-e|--environmentId <ENVIRONMENT_ID>")]
    [Description("The id of the environment.")]
    public Guid EnvironmentId { get; set; }
}