using LanguageExt;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class SubscriptionService(ISubscriptionGateway subscriptionGateway) : ISubscriptionService
{
    public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Topic name is required"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Name is required"));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Webhook url is required"));
        }

        if (eventTypeIds == null || eventTypeIds.Count == 0)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Event type ids are required"));
        }

        if (maxDeliveryAttempts <= 0)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Max delivery attempts should be greater than 0"));
        }

        try
        {
            var subscription = await subscriptionGateway.CreateSubscriptionAsync(topicName, name, description, webhookUrl, eventTypeIds, maxDeliveryAttempts);
            if (subscription == null)
            {
                return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Failed to create subscription"));
            }

            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionSuccessResponse(subscription));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse(e.Message));
        }
    }

    public async Task<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>> DeleteSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse("Subscription id is required"));
        }

        try
        {
            await subscriptionGateway.DeleteSubscriptionAsync(topicName, subscriptionId);
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionSuccessResponse());
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionErrorResponse(e.Message));
        }
    }

    public async Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> GetSubscriptionAsync(string topicName, Guid subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Subscription id is required"));
        }

        try
        {
            var subscription = await subscriptionGateway.GetSubscriptionAsync(topicName, subscriptionId);
            if (subscription == null)
            {
                return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse("Subscription not found"));
            }

            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionSuccessResponse(subscription));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionErrorResponse(e.Message));
        }
    }

    public async Task<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>> GetWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Subscription id is required"));
        }

        try
        {
            var secret = await subscriptionGateway.GetWebhookSecretAsync(topicName, subscriptionId);
            if (secret == null)
            {
                return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse("Webhook secret not found"));
            }

            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretSuccessResponse(secret.WebhookSecret));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretErrorResponse(e.Message));
        }
    }

    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsErrorResponse("Topic name is required"));
        }

        try
        {
            var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsSuccessResponse(subscriptions));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsErrorResponse(e.Message));
        }
    }

    public async Task<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>> RotateWebhookSecretAsync(string topicName, Guid subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Subscription id is required"));
        }

        try
        {
            var secret = await subscriptionGateway.RotateWebhookSecretAsync(topicName, subscriptionId);
            if (secret == null)
            {
                return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse("Failed to rotate webhook secret"));
            }

            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretSuccessResponse(secret.WebhookSecret));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretErrorResponse(e.Message));
        }
    }

    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string? name, string? description, string? webhookUrl, List<Guid?> eventTypeIds, int? maxDeliveryAttempts)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscription id is required"));
        }

        var subscription = await subscriptionGateway.GetSubscriptionAsync(topicName, subscriptionId);

        if (subscription == null)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscription not found"));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            subscription.Name = name;
        }

        if (!string.IsNullOrWhiteSpace(webhookUrl))
        {
            subscription.WebhookUrl = webhookUrl;
        }

        if (eventTypeIds != null && 
            subscription.EventTypes.Select(e => e.Id).ToList() != eventTypeIds.Select(e => e!.Value).ToList())
        {
            subscription.EventTypes = eventTypeIds.Select(e => new EventType { Id = e!.Value }).ToList();
        }

        if (maxDeliveryAttempts is not null)
        {
            if (maxDeliveryAttempts <= 0 && maxDeliveryAttempts > 10)
            {
                return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Max delivery attempts should be a value between 0 and 10"));
            }

            subscription.MaxDeliveryAttempts = maxDeliveryAttempts.Value;
        }

        try
        {
            var updatedSubscription = await subscriptionGateway.UpdateSubscriptionAsync(
                topicName, 
                subscriptionId, 
                subscription.Name, 
                subscription.Description, 
                subscription.WebhookUrl, 
                subscription.EventTypes.Select(e => e.Id).ToList(), 
                subscription.MaxDeliveryAttempts);
            if (updatedSubscription == null)
            {
                return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Failed to update subscription"));
            }

            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionSuccessResponse(updatedSubscription));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse(e.Message));
        }
    }
}