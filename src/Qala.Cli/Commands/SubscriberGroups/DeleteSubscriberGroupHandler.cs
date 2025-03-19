using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.SubscriberGroups;

public record DeleteSubscriberGroupSuccessResponse();
public record DeleteSubscriberGroupErrorResponse(string Message);
public record DeleteSubscriberGroupRequest(string SubscriberGroupName) : IRequest<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>;

public class DeleteSubscriberGroupHandler(ISubscriberGroupService subscriberGroupService)
    : IRequestHandler<DeleteSubscriberGroupRequest, Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>
{
    public async Task<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>> Handle(DeleteSubscriberGroupRequest request, CancellationToken cancellationToken)
    {
        return await subscriberGroupService.DeleteSubscriberGroupAsync(request.SubscriberGroupName)
                .ToAsync()
                .Case switch
        {
            DeleteSubscriberGroupSuccessResponse success => success,
            DeleteSubscriberGroupErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}