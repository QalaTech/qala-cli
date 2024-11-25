using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetWebhookSecretArgument : CommandSettings
{
    [CommandOption("-t|--topic <TOPIC_NAME>")]
    [Description("The name of the topic.")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("-s|--subscription-id <SUBSCRIPTION_ID>")]
    [Description("The id of the subscription.")]
    public Guid SubscriptionId { get; set; } = Guid.Empty;
}