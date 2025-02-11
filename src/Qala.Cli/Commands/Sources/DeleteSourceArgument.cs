using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class DeleteSourceArgument : CommandSettings
{
    [CommandArgument(0, "<SOURCE_NAME>")]
    [Description("The Name of the source to delete.")]
    public string Name { get; set; } = string.Empty;
}