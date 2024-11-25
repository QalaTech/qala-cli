using LanguageExt;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class SubscriptionService(ISubscriptionGateway subscriptionGateway) : ISubscriptionService
{
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

    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsErrorResponse("Topic name is required"));
        }

        var subscriptions = await subscriptionGateway.ListSubscriptionsAsync(topicName);
        return await Task.FromResult<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>(new ListSubscriptionsSuccessResponse(subscriptions));
    }
}