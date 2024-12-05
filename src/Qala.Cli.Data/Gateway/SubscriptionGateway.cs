using System.Net;
using System.Net.Http.Json;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Utils;

namespace Qala.Cli.Data.Gateway;

public class SubscriptionGateway(HttpClient httpClient) : ISubscriptionGateway
{
    public async Task<Subscription?> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"topics/{topicName}/subscriptions", new
            {
                Name = name,
                Description = description,
                WebhookUrl = webhookUrl,
                EventTypeIds = eventTypeIds,
                MaxDeliveryAttempts = maxDeliveryAttempts
            });

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to create subscription");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new Exception("Failed to create subscription");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"topics/{topicName}/subscriptions/{subscriptionId}");

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to delete subscription");
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Subscription?> GetSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions/{subscriptionId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to get subscription");
            }

            return await response.Content.ReadFromJsonAsync<Subscription>() ?? null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<SubscriptionWebhookSecret?> GetWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions/{subscriptionId}/webhook-secret");

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to get webhook secret");
            }

            var content = await response.Content.ReadFromJsonAsync<SubscriptionWebhookSecret>() ?? throw new Exception("Failed to get webhook secret");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<IEnumerable<Subscription?>> ListSubscriptionsAsync(string topicName)
    {
        try
        {
            var response = await httpClient.GetAsync($"topics/{topicName}/subscriptions");

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to list subscriptions");
            }

            return await response.Content.ReadFromJsonAsync<Subscription[]>() ?? [];
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<SubscriptionWebhookSecret?> RotateWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"topics/{topicName}/subscriptions/{subscriptionId}/rotate-webhook-secret", new{});

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to rotate webhook secret");
            }

            var content = await response.Content.ReadFromJsonAsync<SubscriptionWebhookSecret>() ?? throw new Exception("Failed to rotate webhook secret");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Subscription?> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"topics/{topicName}/subscriptions/{subscriptionId}", new
            {
                Name = name,
                Description = description,
                WebhookUrl = webhookUrl,
                EventTypeIds = eventTypeIds,
                MaxDeliveryAttempts = maxDeliveryAttempts
            });

            if (!response.IsSuccessStatusCode)
            {
                await ExceptionsHandler.ThrowExceptionWithProblemDetails(response, "Failed to update subscription");
            }

            var content = await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new Exception("Failed to update subscription");
            return content;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}