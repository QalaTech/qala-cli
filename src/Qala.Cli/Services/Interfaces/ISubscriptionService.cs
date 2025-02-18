using LanguageExt;
using Qala.Cli.Commands.Subscriptions;

namespace Qala.Cli.Services.Interfaces;

public interface ISubscriptionService
{
    Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> ListSubscriptionsAsync(string topicName);
    Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> GetSubscriptionAsync(string topicName, string subscriptionName);
    Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<string> eventTypeNames, int maxDeliveryAttempts);
    Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> UpdateSubscriptionAsync(string topicName, string subscriptionName, string? newName, string? description, string? webhookUrl, List<string>? eventTypeNames, int? maxDeliveryAttempts);
    Task<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>> DeleteSubscriptionAsync(string topicName, string subscriptionName);
    Task<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>> GetWebhookSecretAsync(string topicName, string subscriptionName);
    Task<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>> RotateWebhookSecretAsync(string topicName, string subscriptionName);
}