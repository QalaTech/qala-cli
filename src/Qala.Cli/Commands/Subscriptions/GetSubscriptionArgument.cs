using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetSubscriptionArgument : CommandSettings
{
    [CommandOption("--topic <TOPIC_NAME>")]
    [Description("The name of the topic.")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("--source <SOURCE_NAME>")]
    [Description("The name of the source.")]
    public string SourceName { get; set; } = string.Empty;

    [CommandOption("-s|--subscription <SUBSCRIPTION_ID>")]
    [Description("The id of the subscription.")]
    public Guid SubscriptionId { get; set; }
}