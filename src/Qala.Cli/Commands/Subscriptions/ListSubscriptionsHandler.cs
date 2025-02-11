using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record ListSubscriptionsSuccessResponse(IEnumerable<Subscription> Subscriptions);
public record ListSubscriptionsErrorResponse(string Message);
public record ListSubscriptionsRequest(string? TopicName, string? SourceName) : IRequest<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>;

public class ListSubscriptionsHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<ListSubscriptionsRequest, Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>
{
    public async Task<Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>> Handle(ListSubscriptionsRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new ListSubscriptionsErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = request.TopicName ?? request.SourceName;

        return await subscriptionService.ListSubscriptionsAsync(topicName!)
                .ToAsync()
                .Case switch
        {
            ListSubscriptionsSuccessResponse success => success,
            ListSubscriptionsErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}