using System.ComponentModel;
using Qala.Cli.Utils;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class UpdateSubscriptionArgument : CommandSettings
{
    [CommandArgument(0, "<SUBSCRIPTION_ID>")]
    [Description("The id of the subscription.")]
    public Guid SubscriptionId { get; set; } = Guid.Empty;

    [CommandOption("--topic <TOPIC_NAME>")]
    [Description("The name of the topic.")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("--source <SOURCE_NAME>")]
    [Description("The name of the source.")]
    public string SourceName { get; set; } = string.Empty;

    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the subscription.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the subscription.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-u|--url <WEBHOOK_URL>")]
    [Description("The webhook url of the subscription.")]
    public string WebhookUrl { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENTS_COMMA_SEPERATED_IDS>")]
    [Description("The comma separated list of event type ids.")]
    [TypeConverter(typeof(CommaSeparatedGuidListConverter))]
    public List<Guid> EventTypeIds { get; set; } = [];

    [CommandOption("-m|--max-attempts <MAX_DELIVERY_ATTEMPTS>")]
    [Description("The maximum delivery attempts of the subscription.")]
    public int MaxDeliveryAttempts { get; set; } = 0;
}