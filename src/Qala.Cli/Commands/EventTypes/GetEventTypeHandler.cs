using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.EventTypes;

public record GetEventTypeSuccessResponse(Data.Models.EventType EventType);
public record GetEventTypeErrorResponse(string Message);
public record GetEventTypeRequest(string Name) : IRequest<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>;

public class GetEventTypeHandler(IEventTypeService eventTypeService)
    : IRequestHandler<GetEventTypeRequest, Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>
{
    public async Task<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>> Handle(GetEventTypeRequest request, CancellationToken cancellationToken)
        => await eventTypeService.GetEventTypeAsync(request.Name)
            .ToAsync()
            .Case switch
        {
            GetEventTypeSuccessResponse success => success,
            GetEventTypeErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}
