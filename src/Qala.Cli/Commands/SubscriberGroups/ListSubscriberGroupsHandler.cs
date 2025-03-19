using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.SubscriberGroups;

public record ListSubscriberGroupsSuccessResponse(IEnumerable<SubscriberGroupPrincipal> SubscriberGroups);
public record ListSubscriberGroupsErrorResponse(string Message);
public record ListSubscriberGroupsRequest : IRequest<Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>>;

public class ListSubscriberGroupsHandler(ISubscriberGroupService subscriberGroupService)
    : IRequestHandler<ListSubscriberGroupsRequest, Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>>
{
    public async Task<Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>> Handle(ListSubscriberGroupsRequest request, CancellationToken cancellationToken)
    {
        return await subscriberGroupService.ListSubscriberGroupsAsync()
                .ToAsync()
                .Case switch
        {
            ListSubscriberGroupsSuccessResponse success => success,
            ListSubscriberGroupsErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
    }
}