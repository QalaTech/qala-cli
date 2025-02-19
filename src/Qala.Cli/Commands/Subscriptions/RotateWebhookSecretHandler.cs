using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record RotateWebhookSecretSuccessResponse(string WebhookSecret);
public record RotateWebhookSecretErrorResponse(string Message);
public record RotateWebhookSecretRequest(string? TopicName, string? SourceName, string SubscriptionName) : IRequest<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>;

public class RotateWebhookSecretHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<RotateWebhookSecretRequest, Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>
{
    public async Task<Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>> Handle(RotateWebhookSecretRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new RotateWebhookSecretErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = string.IsNullOrWhiteSpace(request.TopicName) ? request.SourceName : request.TopicName;

        return await subscriptionService.RotateWebhookSecretAsync(topicName!, request.SubscriptionName)
                .ToAsync()
                .Case switch
        {
            RotateWebhookSecretSuccessResponse success => success,
            RotateWebhookSecretErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}