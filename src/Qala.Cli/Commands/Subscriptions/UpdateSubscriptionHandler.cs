using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record UpdateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record UpdateSubscriptionErrorResponse(string Message);
public record UpdateSubscriptionRequest(string? TopicName, string? SourceName, string SubscriptionName, string? NewName, string? Description, string? WebhookUrl, List<string>? EventTypeNames, int? MaxDeliveryAttempts) : IRequest<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>;

public class UpdateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<UpdateSubscriptionRequest, Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>
{
    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> Handle(UpdateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new UpdateSubscriptionErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = string.IsNullOrWhiteSpace(request.TopicName) ? request.SourceName : request.TopicName;

        return await subscriptionService.UpdateSubscriptionAsync(topicName!, request.SubscriptionName, request.NewName, request.Description, request.WebhookUrl, request.EventTypeNames, request.MaxDeliveryAttempts)
                .ToAsync()
                .Case switch
        {
            UpdateSubscriptionSuccessResponse success => success,
            UpdateSubscriptionErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}