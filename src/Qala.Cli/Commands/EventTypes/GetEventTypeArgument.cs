using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class GetEventTypeArgument : CommandSettings
{
    [CommandArgument(0, "<EVENT_TYPE_ID>")]
    [Description("The ID of the event type to retrieve.")]
    public Guid Id { get; set; }
}
