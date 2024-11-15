using Cli.Commands.Create.Subscriptions;
using LanguageExt;

namespace Cli.Services.Interfaces;

internal interface ISubscriptionService
{
    public Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionTopicAsync(string name, string topicName, string description, Guid[] eventTypeIds, string webhookUrl, int maxDeliveryAttempts);
}