using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class ListSubscriptionsArgument : CommandSettings
{
    [CommandOption("-t|--topic <TOPIC_NAME>")]
    public string TopicName { get; set; } = string.Empty;
}