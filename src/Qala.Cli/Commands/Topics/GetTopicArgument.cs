using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class GetTopicArgument : CommandSettings
{
    [CommandArgument(0, "<TOPIC_NAME>")]
    [Description("The Name of the topic to retrieve.")]
    public string Name { get; set; } = string.Empty;
}