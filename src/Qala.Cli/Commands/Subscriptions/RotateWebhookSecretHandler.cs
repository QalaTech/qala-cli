using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record RotateWebhookSecretSuccessResponse(string WebhookSecret);
public record RotateWebhookSecretErrorResponse(string Message);
public record RotateWebhookSecretRequest(string TopicName, Guid SubscriptionId) : IRequest<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>;

public class RotateWebhookSecretHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<RotateWebhookSecretRequest, Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>
{
    public async Task<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>> Handle(RotateWebhookSecretRequest request, CancellationToken cancellationToken)
        => await subscriptionService.RotateWebhookSecretAsync(request.TopicName, request.SubscriptionId)
            .ToAsync()
            .Case switch
            {
                RotateWebhookSecretSuccessResponse success => success,
                RotateWebhookSecretErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}