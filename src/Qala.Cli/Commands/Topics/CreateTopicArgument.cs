using System.ComponentModel;
using Qala.Cli.Utils;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class CreateTopicArgument : CommandSettings
{
    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the topic.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the topic.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--event-type-ids <EVENTS_COMMA_SEPERATED_IDS>")]
    [Description("The comma separated list of event type ids.")]
    [TypeConverter(typeof(CommaSeparatedGuidListConverter))]
    public List<Guid> EventTypeIds { get; set; } = [];
}