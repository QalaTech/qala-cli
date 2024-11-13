namespace Cli.Models;

public class TopicEventType(
    Guid id,
    string type)
{
    public Guid Id { get; set; } = id;
    public string Type { get; set; } = type;
}
