using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class ListSubscriptionsArgument : CommandSettings
{
    [CommandOption("--topic <TOPIC_NAME>")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("--source <SOURCE_NAME>")]
    public string SourceName { get; set; } = string.Empty;
}