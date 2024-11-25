using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record GetSubscriptionSuccessResponse(Data.Models.Subscription Subscription);
public record GetSubscriptionErrorResponse(string Message);
public record GetSubscriptionRequest(string TopicName, Guid SubscriptionId) : IRequest<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>;

public class GetSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<GetSubscriptionRequest, Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>
{
    public async Task<Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>> Handle(GetSubscriptionRequest request, CancellationToken cancellationToken)
        => await subscriptionService.GetSubscriptionAsync(request.TopicName, request.SubscriptionId)
            .ToAsync()
            .Case switch
            {
                GetSubscriptionSuccessResponse success => success,
                GetSubscriptionErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}