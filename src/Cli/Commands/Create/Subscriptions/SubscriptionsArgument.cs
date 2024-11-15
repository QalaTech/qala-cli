using System.ComponentModel;
using Cli.Utils;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Create.Subscriptions;

public class SubscriptionsArgument : CreateArgument
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name of the subscription.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-t|--topic <TOPIC>")]
    [Description("The name of the topic.")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the subscription.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENT_TYPE_IDS>")]
    [TypeConverter(typeof(CommaSeparatedGuidArrayConverter))]
    [Description("The event type IDs to subscribe to.")]
    public Guid[] EventTypeIds { get; set; } = [];

    [CommandOption("-w|--webhook-url <WEBHOOK_URL>")]
    [Description("The URL to send the event to.")]
    public string WebhookUrl { get; set; } = string.Empty;

    [CommandOption("-r|--max-retries <MAX_DELIVERY_ATTEMPTS>")]
    [Description("The maximum number of delivery attempts.")]
    public int MaxDeliveryAttempts { get; set; } = 10;
}