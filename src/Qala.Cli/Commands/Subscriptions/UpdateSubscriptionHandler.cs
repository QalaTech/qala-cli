using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record UpdateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record UpdateSubscriptionErrorResponse(string Message);
public record UpdateSubscriptionRequest(string TopicName, Guid SubscriptionId, string Name, string Description, string WebhookUrl, List<Guid> EventTypeIds, int MaxDeliveryAttempts) : IRequest<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>;

public class UpdateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<UpdateSubscriptionRequest, Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>
{
    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> Handle(UpdateSubscriptionRequest request, CancellationToken cancellationToken)
        => await subscriptionService.UpdateSubscriptionAsync(request.TopicName, request.SubscriptionId, request.Name, request.Description, request.WebhookUrl, request.EventTypeIds, request.MaxDeliveryAttempts)
            .ToAsync()
            .Case switch
            {
                UpdateSubscriptionSuccessResponse success => success,
                UpdateSubscriptionErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}