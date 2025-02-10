using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class UpdateEnvironmentArgument : CommandSettings
{
    [CommandOption("--disableSchemaValidation")]
    [Description("Disable schema validation.")]
    [DefaultValue(false)]
    public bool DisableSchemaValidation { get; set; }
}