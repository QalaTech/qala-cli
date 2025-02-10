using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class SourceGateway(HttpClient client) : ISourceGateway
{
    public async Task<Source?> CreateSourceAsync(string name, string description, SourceType sourceType, string configuration)
    {
        try
        {
            var response = await client.PostAsJsonAsync("sources", new { Name = name, Description = description, SourceType = sourceType, Configuration = configuration });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create source");
            }

            var content = await response.Content.ReadFromJsonAsync<Source>() ?? throw new Exception("Failed to create source");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create topic", e);
        }
    }

    public async Task DeleteSourceAsync(string name)
    {
        try
        {
            var response = await client.DeleteAsync($"sources/{name}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete source");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to delete source", e);
        }
    }

    public async Task<Source?> GetSourceAsync(string name)
    {
        try
        {
            var response = await client.GetAsync($"sources/{name}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get source");
            }

            var content = await response.Content.ReadFromJsonAsync<Source>() ?? throw new Exception("Failed to get source");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get source", e);
        }
    }

    public async Task<IEnumerable<Source>> ListSourcesAsync()
    {
        try
        {
            var response = await client.GetAsync("sources");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to list sources");
            }

            var content = await response.Content.ReadFromJsonAsync<Source[]>() ?? throw new Exception("Failed to list sources");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to list sources", e);
        }
    }

    public async Task<Source?> UpdateSourceAsync(string sourceName, string name, string description, SourceType sourceType, string configuration)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"sources/{sourceName}", new { Name = name, Description = description, SourceType = sourceType, Configuration = configuration });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update source");
            }

            var content = await response.Content.ReadFromJsonAsync<Source>() ?? throw new Exception("Failed to update source");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update source", e);
        }
    }
}