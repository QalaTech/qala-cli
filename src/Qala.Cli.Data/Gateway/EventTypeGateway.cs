using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class EventTypeGateway(HttpClient client) : IEventTypeGateway
{
    public async Task<IEnumerable<EventType>> ListEventTypesAsync()
    {
        try
        {
            var response = await client.GetAsync("events/types");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to list event types");
            }

            var content = await response.Content.ReadFromJsonAsync<EventType[]>() ?? throw new Exception("Failed to list event types");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to list event types", e);
        }
    }
}
