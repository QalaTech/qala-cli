using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class EventTypeGateway(HttpClient client) : IEventTypeGateway
{
    public async Task<EventType?> GetEventTypeAsync(Guid id)
    {
        try
        {
            var response = await client.GetAsync($"events/types/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to get event type:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to get event type");
            }

            return await response.Content.ReadFromJsonAsync<EventType>() ?? null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<IEnumerable<EventType?>> ListEventTypesAsync()
    {
        try
        {
            var response = await client.GetAsync("events/types");

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to list event types:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to list event types");
            }

            return await response.Content.ReadFromJsonAsync<EventType[]>() ?? [];
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
