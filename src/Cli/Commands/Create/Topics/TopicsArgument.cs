using System.ComponentModel;
using Cli.Utils;
using Spectre.Console.Cli;

namespace Cli.Commands.Create.Topics;

public class TopicsArgument : CreateArgument
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name of the topic.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the topic.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENTS>")]
    [TypeConverter(typeof(CommaSeparatedGuidArrayConverter))]
    [Description("The event IDs to associate with the topic.")]
    public Guid[] Events { get; set; } = [];
}

