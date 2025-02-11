using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record GetWebhookSecretSuccessResponse(string WebhookSecret);
public record GetWebhookSecretErrorResponse(string Message);
public record GetWebhookSecretRequest(string? TopicName, string? SourceName, Guid SubscriptionId) : IRequest<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>;

public class GetWebhookSecretHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<GetWebhookSecretRequest, Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>
{
    public async Task<Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>> Handle(GetWebhookSecretRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new GetWebhookSecretErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = request.TopicName ?? request.SourceName;

        return await subscriptionService.GetWebhookSecretAsync(topicName!, request.SubscriptionId)
                .ToAsync()
                .Case switch
        {
            GetWebhookSecretSuccessResponse success => success,
            GetWebhookSecretErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}