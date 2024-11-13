using Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Cli.Commands.Subscriptions;

public record CreateSubscriptionSuccessResponse(string Name, string TopicName, string Description, Guid[] EventTypeIds, string WebhookUrl, int MaxDeliveryAttempts);
public record CreateSubscriptionErrorResponse(string Message);
public record CreateSubscriptionRequest(string Name, string TopicName, string Description, Guid[] EventTypeIds, string WebhookUrl, int MaxDeliveryAttempts) : IRequest<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>;

internal class CreateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<CreateSubscriptionRequest, Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>
    {
        public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> Handle(CreateSubscriptionRequest request, CancellationToken cancellationToken)
            => await subscriptionService.CreateSubscriptionTopicAsync(request.Name, request.TopicName, request.Description, request.EventTypeIds, request.WebhookUrl, request.MaxDeliveryAttempts)
                .ToAsync()
                .Case switch
                {
                    CreateSubscriptionSuccessResponse success => success,
                    CreateSubscriptionErrorResponse error => error,
                    _ => throw new NotImplementedException()
                };   
    }