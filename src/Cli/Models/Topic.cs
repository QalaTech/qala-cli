using static Cli.Utils.Enums;

namespace Cli.Models;

public class Topic(
    Guid id,
    string name,
    string description,
    string provisioningState,
    List<TopicEventType> eventTypes)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string ProvisioningState { get; set; } = provisioningState;
    public List<TopicEventType> EventTypes { get; set; } = eventTypes;
}