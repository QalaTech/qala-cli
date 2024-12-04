using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface IEventTypeGateway
{
    Task<IEnumerable<EventType?>> ListEventTypesAsync();
    Task<EventType?> GetEventTypeAsync(Guid id);
}
