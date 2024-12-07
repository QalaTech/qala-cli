using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record UpdateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record UpdateSubscriptionErrorResponse(string Message);
public record UpdateSubscriptionRequest(string TopicName, string SubscriptionName, string? NewName, string? Description, string? WebhookUrl, List<string>? EventTypeNames, int? MaxDeliveryAttempts) : IRequest<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>;

public class UpdateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<UpdateSubscriptionRequest, Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>
{
    public async Task<Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>> Handle(UpdateSubscriptionRequest request, CancellationToken cancellationToken)
        => await subscriptionService.UpdateSubscriptionAsync(request.TopicName, request.SubscriptionName, request.NewName, request.Description, request.WebhookUrl, request.EventTypeNames, request.MaxDeliveryAttempts)
            .ToAsync()
            .Case switch
            {
                UpdateSubscriptionSuccessResponse success => success,
                UpdateSubscriptionErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}