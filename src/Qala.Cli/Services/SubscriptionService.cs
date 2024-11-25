using LanguageExt;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Gateway.Interfaces;
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

        var subscription = await subscriptionGateway.CreateSubscriptionAsync(topicName, name, description, webhookUrl, eventTypeIds, maxDeliveryAttempts);
        return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionSuccessResponse(subscription));
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

        await subscriptionGateway.DeleteSubscriptionAsync(topicName, subscriptionId);
        return await Task.FromResult<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>(new DeleteSubscriptionSuccessResponse());
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

        var subscription = await subscriptionGateway.GetSubscriptionAsync(topicName, subscriptionId);
        return await Task.FromResult<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>(new GetSubscriptionSuccessResponse(subscription));
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

        var secret = await subscriptionGateway.GetWebhookSecretAsync(topicName, subscriptionId);
        return await Task.FromResult<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>(new GetWebhookSecretSuccessResponse(secret));
    }

    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsErrorResponse("Topic name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsSuccessResponse(subscriptions));
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

        var secret = await subscriptionGateway.RotateWebhookSecretAsync(topicName, subscriptionId);
        return await Task.FromResult<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>(new RotateWebhookSecretSuccessResponse(secret));
    }

    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Topic name is required"));
        }

        if (subscriptionId == Guid.Empty)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Subscription id is required"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Name is required"));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Webhook url is required"));
        }

        if (eventTypeIds == null || eventTypeIds.Count == 0)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Event type ids are required"));
        }

        if (maxDeliveryAttempts <= 0)
        {
            return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionErrorResponse("Max delivery attempts should be greater than 0"));
        }

        var subscription = await subscriptionGateway.UpdateSubscriptionAsync(topicName, subscriptionId, name, description, webhookUrl, eventTypeIds, maxDeliveryAttempts);
        return await Task.FromResult<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>(new UpdateSubscriptionSuccessResponse(subscription));
    }
}