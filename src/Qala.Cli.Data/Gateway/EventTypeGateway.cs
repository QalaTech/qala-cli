using System.Net;
using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Utils;

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
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to get event type");
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
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to list event types");
            }

            return await response.Content.ReadFromJsonAsync<EventType[]>() ?? [];
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
