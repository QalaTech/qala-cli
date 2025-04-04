using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Subscriptions;

public record DeleteSubscriptionSuccessResponse();
public record DeleteSubscriptionErrorResponse(string Message);
public record DeleteSubscriptionRequest(string? TopicName, string? SourceName, string SubscriptionName) : IRequest<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>;

public class DeleteSubscriptionHandler(ISubscriptionService subscriptionService)
    : IRequestHandler<DeleteSubscriptionRequest, Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>
{
    public async Task<Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>> Handle(DeleteSubscriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.TopicName is null && request.SourceName is null)
        {
            return new DeleteSubscriptionErrorResponse("Either Topic name or Source name must be provided.");
        }

        var topicName = string.IsNullOrWhiteSpace(request.TopicName) ? request.SourceName : request.TopicName;

        return await subscriptionService.DeleteSubscriptionAsync(topicName!, request.SubscriptionName)
                .ToAsync()
                .Case switch
        {
            DeleteSubscriptionSuccessResponse success => success,
            DeleteSubscriptionErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}
