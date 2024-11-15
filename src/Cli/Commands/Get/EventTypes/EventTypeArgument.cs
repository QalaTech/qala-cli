using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Get.EventTypes;

public class EventTypesArgument : GetArgument
{
    [CommandArgument(0, "[EVENT_TYPE_ID]")]
    [Description("The ID of the event type.")]
    public string? Id { get; set; }
}