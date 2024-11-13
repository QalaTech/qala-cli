using Spectre.Console.Cli;

namespace Cli.Commands.Topics;

public class CreateTopicArgument : CommandSettings
{
    [CommandArgument(0, "<NAME>")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENTS>")]
    public Guid[] Events { get; set; } = [];
}