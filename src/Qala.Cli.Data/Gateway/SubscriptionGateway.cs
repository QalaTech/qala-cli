using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class SubscriptionGateway(HttpClient httpClient) : ISubscriptionGateway
{
    public Task<Subscription> CreateSubscriptionAsync(string topicName, Guid subscriptionId, string description, List<Guid> eventTypeIds)
    {
        throw new NotImplementedException();
    }

    public async Task<Subscription> GetSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions/{subscriptionId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get subscription");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new Exception("Failed to get subscription");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get subscription", e);
        }
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string topicName)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to list subscriptions");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription[]>() ?? throw new Exception("Failed to list subscriptions");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to list subscriptions", e);
        }
    }

    public Task<Subscription> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string description, List<Guid> eventTypeIds)
    {
        throw new NotImplementedException();
    }
}