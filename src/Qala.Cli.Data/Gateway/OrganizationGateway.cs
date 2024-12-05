using System.Net.Http.Json;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Utils;
using System.Net;

namespace Qala.Cli.Gateway;

public class OrganizationGateway(HttpClient client) : IOrganizationGateway
{
    public async Task<Organization?> GetOrganizationAsync()
    {
        try
        {
            var response = await client.GetAsync("organization");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to get organization");
            }

            return await response.Content.ReadFromJsonAsync<Organization>() ?? null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}