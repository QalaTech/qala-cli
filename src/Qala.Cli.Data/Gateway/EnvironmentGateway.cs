using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class EnvironmentGateway(HttpClient client) : IEnvironmentGateway
{
    public async Task<Models.Environment> CreateEnvironmentAsync(Models.Environment environment)
    {
        try
        {
            var response = await client.PostAsJsonAsync("environments", environment);

            if (!response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (data != null && data.Errors != null)
                {
                    throw new Exception("Failed to create environment:" + string.Join(", ", data.Errors.Select(x => x.Reason)));
                }
                
                throw new Exception("Failed to create environment");
            }

            var content = await response.Content.ReadFromJsonAsync<Models.Environment>() ?? throw new Exception("Failed to create environment");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
