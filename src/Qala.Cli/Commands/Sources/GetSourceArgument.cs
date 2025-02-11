using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class GetSourceArgument : CommandSettings
{
    [CommandArgument(0, "<SOURCE_NAME>")]
    [Description("The Name of the source to retrieve.")]
    public string Name { get; set; } = string.Empty;
}