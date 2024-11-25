using LanguageExt;
using Qala.Cli.Commands.Subscriptions;

namespace Qala.Cli.Services.Interfaces;

public interface ISubscriptionService
{
    Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName);
    Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> GetSubscriptionAsync(string topicName, Guid subscriptionId);
}