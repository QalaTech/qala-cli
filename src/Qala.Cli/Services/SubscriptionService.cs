using LanguageExt;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class SubscriptionService(ISubscriptionGateway subscriptionGateway, IEventTypeGateway eventTypeGateway) : ISubscriptionService
{
    public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionAsync(string topicType, string topicName, string name, string description, string webhookUrl, List<string> eventTypeNames, int maxDeliveryAttempts, string? audience)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Name is required"));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Description is required"));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Webhook url is required"));
        }

        if (topicType == "Publisher" && (eventTypeNames == null || eventTypeNames.Count == 0))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Event type ids are required"));
        }

        if (maxDeliveryAttempts < 0 || maxDeliveryAttempts > 10)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Max delivery attempts should be between 0 and 10"));
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            audience = string.Empty;
        }

        List<Guid> eventTypeIds = [];
        if (topicType == "Publisher")
        {
            var eventTypes = await eventTypeGateway.ListEventTypesAsync();
            if (eventTypes == null || !eventTypes.Any())
            {
                return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Event types not found"));
            }

            eventTypeIds = eventTypes
                .Where(e => e?.Type != null && eventTypeNames.Contains(e.Type))
                .Select(e => e?.Id)
                .Where(id => id.HasValue)
                .Select(id => id.GetValueOrDefault())
                .ToList();

            if (eventTypeIds == null || eventTypeIds.Count == 0)
            {
                return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Event types not found"));
            }
        }

        var subscription = await subscriptionGateway.CreateSubscriptionAsync(topicName, name, description, webhookUrl, eventTypeIds, maxDeliveryAttempts, audience);
        if (subscription == null)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Failed to create subscription"));
        }

        return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionSuccessResponse(subscription));
    }

    public async Task<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>> DeleteSubscriptionAsync(string topicName, string subscriptionName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Subscription name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        if (subscriptions == null || !subscriptions.Any())
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Subscriptions not found"));
        }

        var subscriptionId = subscriptions.FirstOrDefault(s => s?.Name == subscriptionName)?.Id;

        if (subscriptionId == null || subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Subscription not found"));
        }

        await subscriptionGateway.DeleteSubscriptionAsync(topicName, (Guid)subscriptionId);
        return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionSuccessResponse());
    }

    public async Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> GetSubscriptionAsync(string topicName, string subscriptionName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Subscription name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        if (subscriptions == null || !subscriptions.Any())
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Subscriptions not found"));
        }

        var subscription = subscriptions.FirstOrDefault(s => s?.Name == subscriptionName);

        if (subscription == null)
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Subscription not found"));
        }

        return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionSuccessResponse(subscription));
    }

    public async Task<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>> GetWebhookSecretAsync(string topicName, string subscriptionName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Subscription name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        if (subscriptions == null || !subscriptions.Any())
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Subscriptions not found"));
        }

        var subscriptionId = subscriptions.FirstOrDefault(s => s?.Name == subscriptionName)?.Id;

        if (subscriptionId == null || subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Subscription not found"));
        }

        var secret = await subscriptionGateway.GetWebhookSecretAsync(topicName, (Guid)subscriptionId);
        if (secret == null)
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Webhook secret not found"));
        }

        return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretSuccessResponse(secret));
    }

    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsErrorResponse("Either Topic name or Source name must be provided"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsSuccessResponse(subscriptions));
    }

    public async Task<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>> RotateWebhookSecretAsync(string topicName, string subscriptionName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Subscription name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        if (subscriptions == null || !subscriptions.Any())
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Subscriptions not found"));
        }

        var subscriptionId = subscriptions.FirstOrDefault(s => s?.Name == subscriptionName)?.Id;

        if (subscriptionId == null || subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Subscription not found"));
        }

        var secret = await subscriptionGateway.RotateWebhookSecretAsync(topicName, (Guid)subscriptionId);
        if (secret == null)
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Failed to rotate webhook secret"));
        }

        return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretSuccessResponse(secret));
    }

    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> UpdateSubscriptionAsync(string topicType, string topicName, string subscriptionName, string? newName, string? description, string? webhookUrl, List<string>? eventTypeNames, int? maxDeliveryAttempts, string? audience)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Either Topic name or Source name must be provided"));
        }

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscription name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        if (subscriptions == null || !subscriptions.Any())
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscriptions not found"));
        }

        var subscription = subscriptions.FirstOrDefault(s => s?.Name == subscriptionName);

        if (subscription == null)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscription not found"));
        }

        if (!string.IsNullOrWhiteSpace(newName))
        {
            subscription.Name = newName;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            subscription.Description = description;
        }

        if (!string.IsNullOrWhiteSpace(webhookUrl))
        {
            subscription.WebhookUrl = webhookUrl;
        }

        if (eventTypeNames != null && eventTypeNames.Count != 0)
        {
            var eventTypes = await eventTypeGateway.ListEventTypesAsync();
            if (eventTypes == null || !eventTypes.Any())
            {
                return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Event types not found"));
            }

            if (subscription.EventTypes.Select(e => e.Type).ToList() != eventTypeNames)
            {
                var newEventTypeIds = eventTypes
                    .Where(e => e?.Type != null && eventTypeNames.Contains(e.Type))
                    .Select(e => e?.Id)
                    .Where(id => id.HasValue)
                    .Select(id => id.GetValueOrDefault())
                    .ToList();

                subscription.EventTypes = newEventTypeIds.Select(id => new EventType { Id = id }).ToList();
            }
        }

        if (maxDeliveryAttempts != null && maxDeliveryAttempts != 0)
        {
            if (maxDeliveryAttempts <= 0 || maxDeliveryAttempts > 10)
            {
                return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Max delivery attempts should be a value between 0 and 10"));
            }

            subscription.MaxDeliveryAttempts = (int)maxDeliveryAttempts;
        }

        if (!string.IsNullOrWhiteSpace(audience))
        {
            subscription.Audience = audience;
        }

        var updatedSubscription = await subscriptionGateway.UpdateSubscriptionAsync(
                topicType,
                topicName,
                subscription.Id,
                subscription.Name,
                subscription.Description,
                subscription.WebhookUrl,
                subscription.EventTypes.Select(e => e.Id).ToList(),
                subscription.MaxDeliveryAttempts,
                subscription.Audience ?? string.Empty);

        if (updatedSubscription == null)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Failed to update subscription"));
        }

        return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionSuccessResponse(updatedSubscription));
    }
}