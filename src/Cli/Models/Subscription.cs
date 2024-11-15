using static Cli.Utils.Enums;

namespace Qala.Cli.Models;

public class Subscription(
    Guid id,
    string name,
    string description,
    string webhookUrl,
    string provisioningState,
    List<SubscriptionEventType> eventTypes,
    int maxDeliveryAttempts,
    string topicName,
    int deadlettersCount)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string WebhookUrl { get; set; } = webhookUrl;
    public string ProvisioningState { get; set; } = provisioningState;
    public List<SubscriptionEventType> EventTypes { get; set; } = eventTypes;
    public int MaxDeliveryAttempts { get; set; } = maxDeliveryAttempts;
    public string TopicName { get; set; } = topicName;
    public int DeadlettersCount { get; set; } = deadlettersCount;
}