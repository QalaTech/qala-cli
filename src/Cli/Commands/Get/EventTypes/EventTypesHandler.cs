using Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Cli.Commands.Get.EventTypes;

public record EventTypesResponse(Models.EventType[] EventTypes);
public record EventTypesErrorResponse(string Message);
public record EventTypesRequest : IRequest<Either<EventTypesErrorResponse, EventTypesResponse>>;

internal class GetAllEventTypesHandler(IEventTypeService eventTypeService)
    : IRequestHandler<EventTypesRequest, Either<EventTypesErrorResponse, EventTypesResponse>>
{
    public async Task<Either<EventTypesErrorResponse, EventTypesResponse>> Handle(EventTypesRequest request, CancellationToken cancellationToken)
        => await eventTypeService.GetAllEventTypesAsync()
            .ToAsync()
            .Case switch
            {
                EventTypesResponse success => success,
                EventTypesErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}