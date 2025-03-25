using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.EventTypes;

public record CreateEventTypesSuccessResponse(List<Data.Models.EventType> EventTypes);
public record CreateEventTypesErrorResponse(string Message);
public record CreateEventTypesRequest(string ImportFilePath) : IRequest<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>>;

public class CreateEventTypesHandler(IEventTypeService eventTypeService)
    : IRequestHandler<CreateEventTypesRequest, Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>>
{
    public async Task<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>> Handle(CreateEventTypesRequest request, CancellationToken cancellationToken)
     => await eventTypeService.CreateEventTypesAsync(request.ImportFilePath)
        .ToAsync()
        .Case switch
     {
         CreateEventTypesSuccessResponse success => success,
         CreateEventTypesErrorResponse error => error,
         _ => throw new NotImplementedException()
     };
}