using System.Data;
using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class TopicGateway(HttpClient client) : ITopicGateway
{
    public async Task<Topic?> CreateTopicAsync(string name, string description, List<Guid> eventTypeIds)
    {
        try
        {
            var response = await client.PostAsJsonAsync("topics", new { Name = name, Description = description, EventTypeIds = eventTypeIds });

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception(string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to create topic");
            }

            var content = await response.Content.ReadFromJsonAsync<Topic>() ?? throw new Exception("Failed to create topic");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Topic?> GetTopicAsync(string name)
    {
        try
        {
            var response = await client.GetAsync($"topics/{name}");

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to get topic:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to get topic");
            }

            return await response.Content.ReadFromJsonAsync<Topic>() ?? null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<IEnumerable<Topic?>> ListTopicsAsync()
    {
        try
        {
            var response = await client.GetAsync("topics");

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to list topics:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to list topics");
            }

            return await response.Content.ReadFromJsonAsync<Topic[]>() ?? [];
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Topic?> UpdateTopicAsync(string name, string newName, string description, List<Guid> eventTypeIds)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"topics/{name}", new { Name = newName, Description = description, EventTypeIds = eventTypeIds });

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to update topic:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to update topic");
            }

            var content = await response.Content.ReadFromJsonAsync<Topic>() ?? throw new Exception("Failed to update topic");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
