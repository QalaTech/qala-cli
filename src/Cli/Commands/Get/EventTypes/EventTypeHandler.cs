using Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Qala.Cli.Commands.Get.EventTypes;

public record EventTypeResponse(Models.EventType EventType);
public record EventTypeErrorResponse(string Message);
public record EventTypeRequest(string Id) : IRequest<Either<EventTypeErrorResponse, EventTypeResponse>>;

internal class GetEventTypeHandler(IEventTypeService eventTypeService)
    : IRequestHandler<EventTypeRequest, Either<EventTypeErrorResponse, EventTypeResponse>>
{
    public async Task<Either<EventTypeErrorResponse, EventTypeResponse>> Handle(EventTypeRequest request, CancellationToken cancellationToken)
        => await eventTypeService.GetEventTypeAsync(request.Id)
            .ToAsync()
            .Case switch
            {
                EventTypeResponse success => success,
                EventTypeErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}