using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record CreateSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record CreateSubscriptionErrorResponse(string Message);
public record CreateSubscriptionRequest(string? TopicName, string? SourceName, string Name, string Description, string WebhookUrl, List<string> EventTypeNames, int MaxDeliveryAttempts, string Audience) : IRequest<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>;

public class CreateSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<CreateSubscriptionRequest, Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>
{
    public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> Handle(CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new CreateSubscriptionErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = string.IsNullOrWhiteSpace(request.TopicName) ? request.SourceName : request.TopicName;

        var topicType = string.IsNullOrWhiteSpace(request.TopicName) ? "Source" : "Publisher";

        return await subscriptionService.CreateSubscriptionAsync(topicType, topicName!, request.Name, request.Description, request.WebhookUrl, request.EventTypeNames, request.MaxDeliveryAttempts, request.Audience)
                .ToAsync()
                .Case switch
        {
            CreateSubscriptionSuccessResponse success => success,
            CreateSubscriptionErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}