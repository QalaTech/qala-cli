using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.SubscriberGroups;

public record CreateSubscriberGroupSuccessResponse(Data.Models.SubscriberGroupPrincipal SubscriberGroup);
public record CreateSubscriberGroupErrorResponse(string Message);
public record CreateSubscriberGroupRequest(string Name, string Description, List<string> Topics, string Audience) : IRequest<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>;

public class CreateSubscriberGroupHandler(ISubscriberGroupService subscriberGroupService)
    : IRequestHandler<CreateSubscriberGroupRequest, Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>
{
    public async Task<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>> Handle(CreateSubscriberGroupRequest request, CancellationToken cancellationToken)
    {
        return await subscriberGroupService.CreateSubscriberGroupAsync(request.Name, request.Description, request.Topics, request.Audience)
                .ToAsync()
                .Case switch
        {
            CreateSubscriberGroupSuccessResponse success => success,
            CreateSubscriberGroupErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}