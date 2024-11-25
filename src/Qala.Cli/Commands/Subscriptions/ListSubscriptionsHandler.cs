using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record ListSubscriptionsSuccessResponse(IEnumerable<Subscription> Subscriptions);
public record ListSubscriptionsErrorResponse(string Message);
public record ListSubscriptionsRequest(string TopicName) : IRequest<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>;

public class ListSubscriptionsHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<ListSubscriptionsRequest, Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>
{
    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> Handle(ListSubscriptionsRequest request, CancellationToken cancellationToken)
        => await subscriptionService.ListSubscriptionsAsync(request.TopicName)
            .ToAsync()
            .Case switch
            {
                ListSubscriptionsSuccessResponse success => success,
                ListSubscriptionsErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}