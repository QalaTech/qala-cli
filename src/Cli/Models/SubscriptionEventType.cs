namespace Cli.Models;

public class SubscriptionEventType(
    Guid id,
    string type)
{
    public Guid Id { get; set; } = id;
    public string Type { get; set; } = type;
}
