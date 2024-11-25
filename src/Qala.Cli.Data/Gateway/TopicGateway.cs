using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class TopicGateway(HttpClient client) : ITopicGateway
{
    public async Task<Topic> GetTopicAsync(string name)
    {
        try
        {
            var response = await client.GetAsync($"topics/{name}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get topic");
            }

            var content = await response.Content.ReadFromJsonAsync<Topic>() ?? throw new Exception("Failed to get topic");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get topic", e);
        }
    }

    public async Task<IEnumerable<Topic>> ListTopicsAsync()
    {
        try
        {
            var response = await client.GetAsync("topics");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to list topics");
            }

            var content = await response.Content.ReadFromJsonAsync<Topic[]>() ?? throw new Exception("Failed to list topics");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to list topics", e);
        }
    }
}
