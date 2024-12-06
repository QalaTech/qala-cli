using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.EventTypes;

public record ListEventTypesSuccessResponse(IEnumerable<Data.Models.EventType?> EventTypes);
public record ListEventTypesErrorResponse(string Message);
public record ListEventTypesRequest : IRequest<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>;

public class ListEventTypesHandler(IEventTypeService eventTypeService)
    : IRequestHandler<ListEventTypesRequest, Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>
{
    public async Task<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>> Handle(ListEventTypesRequest request, CancellationToken cancellationToken)
        => await eventTypeService.ListEventTypesAsync()
            .ToAsync()
            .Case switch
            {
                ListEventTypesSuccessResponse success => success,
                ListEventTypesErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}
