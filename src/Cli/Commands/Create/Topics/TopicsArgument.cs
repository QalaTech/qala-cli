using System.ComponentModel;
using Cli.Utils;
using Spectre.Console.Cli;

namespace Cli.Commands.Create.Topics;

public class TopicsArgument : CreateArgument
{
    [CommandArgument(0, "<NAME>")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENTS>")]
    [TypeConverter(typeof(CommaSeparatedGuidArrayConverter))]
    public Guid[] Events { get; set; } = [];
}

