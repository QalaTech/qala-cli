using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record UpdateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record UpdateSubscriptionErrorResponse(string Message);
public record UpdateSubscriptionRequest(string? TopicName, string? SourceName, Guid SubscriptionId, string Name, string Description, string WebhookUrl, List<Guid> EventTypeIds, int MaxDeliveryAttempts) : IRequest<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>;

public class UpdateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<UpdateSubscriptionRequest, Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>
{
    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> Handle(UpdateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new UpdateSubscriptionErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = request.TopicName ?? request.SourceName;
        return await subscriptionService.UpdateSubscriptionAsync(topicName!, request.SubscriptionId, request.Name, request.Description, request.WebhookUrl, request.EventTypeIds, request.MaxDeliveryAttempts)
                .ToAsync()
                .Case switch
        {
            UpdateSubscriptionSuccessResponse success => success,
            UpdateSubscriptionErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}