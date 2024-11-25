using LanguageExt;
using Qala.Cli.Commands.Subscriptions;

namespace Qala.Cli.Services.Interfaces;

public interface ISubscriptionService
{
    Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName);
    Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> GetSubscriptionAsync(string topicName, Guid subscriptionId);
    Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts);
    Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts);
    Task<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>> DeleteSubscriptionAsync(string topicName, Guid subscriptionId);
}