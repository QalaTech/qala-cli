namespace Qala.Cli.Models;

public class EventType(
    Guid id,
    string type)
{
    public Guid Id { get; set; } = id;
    public string Type { get; set; } = type;
}
