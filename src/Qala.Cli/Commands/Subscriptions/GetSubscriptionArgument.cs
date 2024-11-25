using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetSubscriptionArgument : CommandSettings
{
    [CommandOption("-t|--topic <TOPIC_NAME>")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("-s|--subscription <SUBSCRIPTION_ID>")]
    public Guid SubscriptionId { get; set; }
}