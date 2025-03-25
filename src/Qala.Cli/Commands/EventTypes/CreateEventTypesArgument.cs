using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class CreateEventTypeArgument : CommandSettings
{
    [CommandOption("-i|--import <IMPORT_FILE_PATH>")]
    [Description("The path to the file containing the event type definition.")]
    public string ImportFilePath { get; set; } = string.Empty;
}