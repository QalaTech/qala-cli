using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface ITopicGateway
{
    Task<IEnumerable<Topic?>> ListTopicsAsync();
    Task<Topic?> GetTopicAsync(string name);
    Task<Topic?> CreateTopicAsync(string name, string description, List<Guid> eventTypeIds);
    Task<Topic?> UpdateTopicAsync(string name, string description, List<Guid> eventTypeIds);
}
