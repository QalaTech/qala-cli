using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record CreateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record CreateSubscriptionErrorResponse(string Message);
public record CreateSubscriptionRequest(string TopicName, string Name, string Description, string WebhookUrl, List<Guid> EventTypeIds, int MaxDeliveryAttempts) : IRequest<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>;

public class CreateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<CreateSubscriptionRequest, Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>
{
    public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> Handle(CreateSubscriptionRequest request, CancellationToken cancellationToken)
        => await subscriptionService.CreateSubscriptionAsync(request.TopicName, request.Name, request.Description, request.WebhookUrl, request.EventTypeIds, request.MaxDeliveryAttempts)
            .ToAsync()
            .Case switch
            {
                CreateSubscriptionSuccessResponse success => success,
                CreateSubscriptionErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}