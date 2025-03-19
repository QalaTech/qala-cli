using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.SubscriberGroups;

public record GetSubscriberGroupSuccessResponse(Data.Models.SubscriberGroupPrincipal SubscriberGroup);
public record GetSubscriberGroupErrorResponse(string Message);
public record GetSubscriberGroupRequest(string SubscriberGroupName) : IRequest<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>;

public class GetSubscriberGroupHandler(ISubscriberGroupService subscriberGroupService)
    : IRequestHandler<GetSubscriberGroupRequest, Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>
{
    public async Task<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>> Handle(GetSubscriberGroupRequest request, CancellationToken cancellationToken)
    {
        return await subscriberGroupService.GetSubscriberGroupAsync(request.SubscriberGroupName)
                .ToAsync()
                .Case switch
        {
            GetSubscriberGroupSuccessResponse success => success,
            GetSubscriberGroupErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}