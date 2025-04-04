using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway;

public class SubscriptionGateway(HttpClient httpClient) : ISubscriptionGateway
{
    public async Task<Subscription?> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts, string audience)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"topics/{topicName}/subscriptions", new
            {
                Name = name,
                Description = description,
                WebhookUrl = webhookUrl,
                EventTypeIds = eventTypeIds,
                MaxDeliveryAttempts = maxDeliveryAttempts,
                Audience = audience
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create subscription");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new Exception("Failed to create subscription");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create subscription", e);
        }
    }

    public async Task DeleteSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"topics/{topicName}/subscriptions/{subscriptionId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete subscription");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to delete subscription", e);
        }
    }

    public async Task<Subscription?> GetSubscriptionAsync(string topicName, Guid subscriptionId)
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

    public async Task<string?> GetWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions/{subscriptionId}/webhook-secret");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get webhook secret");
            }

            var content = await response.Content.ReadAsStringAsync() ?? throw new Exception("Failed to get webhook secret");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get webhook secret", e);
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

    public async Task<string?> RotateWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"topics/{topicName}/subscriptions/{subscriptionId}/rotate-webhook-secret", new { });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to rotate webhook secret");
            }

            var content = await response.Content.ReadAsStringAsync() ?? throw new Exception("Failed to rotate webhook secret");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to rotate webhook secret", e);
        }
    }

    public async Task<Subscription?> UpdateSubscriptionAsync(string topicType, string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts, string audience)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"topics/{topicName}/subscriptions/{subscriptionId}", new
            {
                Name = name,
                Description = description,
                WebhookUrl = webhookUrl,
                EventTypeIds = eventTypeIds,
                MaxDeliveryAttempts = maxDeliveryAttempts,
                TopicType = topicType,
                Audience = audience
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update subscription");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new Exception("Failed to update subscription");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update subscription", e);
        }
    }
}