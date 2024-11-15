using System.ComponentModel;
using Cli.Utils;
using Spectre.Console.Cli;

namespace Cli.Commands.Create.Subscriptions;

public class SubscriptionsArgument : CreateArgument
{
    [CommandArgument(0, "<NAME>")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-t|--topic <TOPIC>")]
    public string TopicName { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-e|--events <EVENT_TYPE_IDS>")]
    [TypeConverter(typeof(CommaSeparatedGuidArrayConverter))]
    public Guid[] EventTypeIds { get; set; } = [];

    [CommandOption("-w|--webhook-url <WEBHOOK_URL>")]
    public string WebhookUrl { get; set; } = string.Empty;

    [CommandOption("-r|--max-retries <MAX_DELIVERY_ATTEMPTS>")]
    public int MaxDeliveryAttempts { get; set; } = 10;
}