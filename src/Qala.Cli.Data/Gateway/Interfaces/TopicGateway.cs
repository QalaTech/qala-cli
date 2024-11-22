using System.Net.Http.Json;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public class TopicGateway(HttpClient client) : ITopicGateway
{
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
