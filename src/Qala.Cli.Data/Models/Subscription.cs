namespace Qala.Cli.Data.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string ProvisioningState { get; set; } = string.Empty;
    public List<EventType> EventTypes { get; set; } = [];
    public int MaxDeliveryAttempts { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int DeadletterCount { get; set; }
}