using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.SubscriberGroups;

public record UpdateSubscriberGroupSuccessResponse(Data.Models.SubscriberGroupPrincipal SubscriberGroup);
public record UpdateSubscriberGroupErrorResponse(string Message);
public record UpdateSubscriberGroupRequest(string SubscriberGroupName, string? NewName, string? Description, List<string>? Topics, string? Audience) : IRequest<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>;

public class UpdateSubscriberGroupHandler(ISubscriberGroupService subscriberGroupService)
    : IRequestHandler<UpdateSubscriberGroupRequest, Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>
{
    public async Task<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>> Handle(UpdateSubscriberGroupRequest request, CancellationToken cancellationToken)
    {
        return await subscriberGroupService.UpdateSubscriberGroupAsync(request.SubscriberGroupName, request.NewName, request.Description, request.Topics, request.Audience)
                .ToAsync()
                .Case switch
        {
            UpdateSubscriberGroupSuccessResponse success => success,
            UpdateSubscriberGroupErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}