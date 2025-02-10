using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;

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
                throw new Exception("Failed to create environment");
            }

            var content = await response.Content.ReadFromJsonAsync<Models.Environment>() ?? throw new Exception("Failed to create environment");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create environment", e);
        }
    }

    public async Task<Models.Environment?> UpdateEnvironmentAsync(Guid environmentId, string environemntName, bool disableSchemaValidation)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"environments/{environmentId}", new { Name = environemntName, IsSchemaValidationEnabled = !disableSchemaValidation });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update environment");
            }

            var content = await response.Content.ReadFromJsonAsync<Models.Environment>() ?? throw new Exception("Failed to update environment");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update environment", e);
        }
    }
}
