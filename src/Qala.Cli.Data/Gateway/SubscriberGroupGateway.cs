using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class SubscriberGroupGateway(HttpClient httpClient) : ISubscriberGroupGateway
{
    public async Task<SubscriberGroupPrincipal?> CreateSubscriberGroupAsync(string name, string description, List<Permission> permissions, string audience)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("security/subscriber-groups", new
            {
                Name = name,
                Description = description,
                AvailablePermissions = permissions,
                Audience = audience
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create subscriber group");
            }

            var content = await response.Content.ReadFromJsonAsync<SubscriberGroupPrincipal>() ?? throw new Exception("Failed to create subscriber group");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create subscriber group", e);
        }
    }

    public async Task DeleteSubscriberGroupAsync(Guid subscriberGroupId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"security/subscriber-groups/{subscriberGroupId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete subscriber group");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to delete subscriber group", e);
        }
    }

    public async Task<SubscriberGroupPrincipal> GetSubscriberGroupAsync(Guid subscriberGroupId)
    {
        try
        {
            var response = await httpClient.GetAsync($"security/subscriber-groups/{subscriberGroupId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get subscriber group");
            }

            var content = await response.Content.ReadFromJsonAsync<SubscriberGroupPrincipal>() ?? throw new Exception("Failed to get subscriber group");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get subscriber group", e);
        }
    }

    public async Task<List<SubscriberGroupPrincipal>> ListSubscriberGroupsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("security/subscriber-groups");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to list subscriber groups");
            }

            var content = await response.Content.ReadFromJsonAsync<List<SubscriberGroupPrincipal>>() ?? throw new Exception("Failed to list subscriber groups");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to list subscriber groups", e);
        }
    }

    public async Task UpdateSubscriberGroupAsync(Guid subscriberGroupId, string? name, string? description, List<Permission>? permissions, string? audience)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"security/subscriber-groups/{subscriberGroupId}", new
            {
                Name = name,
                Description = description,
                AvailablePermissions = permissions,
                Audience = audience
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update subscriber group");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update subscriber group", e);
        }
    }
}