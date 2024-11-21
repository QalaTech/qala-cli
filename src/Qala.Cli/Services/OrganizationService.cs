using System.Net.Http.Json;
using Qala.Cli.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class OrganizationService(HttpClient client) : IOrganizationService
{
    public async Task<Organization> GetOrganizationAsync()
    {
        try
        {
            var response = await client.GetAsync("organization");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get organization");
            }

            var content = await response.Content.ReadFromJsonAsync<Organization>() ?? throw new Exception("Failed to get organization");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get organization", e);
        }
    }
}