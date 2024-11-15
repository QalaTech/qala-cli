using Spectre.Console.Cli;

namespace Cli.Commands.Get.EventTypes;

public class EventTypesArgument : GetArgument
{
    [CommandArgument(0, "[EVENT_TYPE_ID]")]
    public string? Id { get; set; }
}