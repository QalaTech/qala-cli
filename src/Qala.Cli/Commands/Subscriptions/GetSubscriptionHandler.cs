using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record GetSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record GetSubscriptionErrorResponse(string Message);
public record GetSubscriptionRequest(string? TopicName, string? SourceName, string SubscriptionName) : IRequest<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>;

public class GetSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<GetSubscriptionRequest, Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>
{
    public async Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> Handle(GetSubscriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new GetSubscriptionErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = request.TopicName ?? request.SourceName;

        return await subscriptionService.GetSubscriptionAsync(topicName!, request.SubscriptionName)
                .ToAsync()
                .Case switch
        {
            GetSubscriptionSuccessResponse success => success,
            GetSubscriptionErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}