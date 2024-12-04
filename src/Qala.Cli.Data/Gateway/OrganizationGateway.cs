using System.Net.Http.Json;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Gateway.Interfaces;

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
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to get organization:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to get organization");
            }

            return await response.Content.ReadFromJsonAsync<Organization>() ?? null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}