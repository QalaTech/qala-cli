namespace Qala.Cli.Data.Models;

public class Topic
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProvisioningState { get; set; } = string.Empty;
    public List<EventType> EventTypes { get; set; } = [];
}
