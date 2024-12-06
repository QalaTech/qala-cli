using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class GetEventTypeArgument : CommandSettings
{
    [CommandArgument(0, "<EVENT_TYPE_NAME>")]
    [Description("The Name of the event type to retrieve.")]
    public string Name { get; set; } = string.Empty;
}
