using Qala.Cli.Commands.EventTypes;
using LanguageExt;

namespace Qala.Cli.Services.Interfaces;

public interface IEventTypeService
{
    Task<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>> ListEventTypesAsync();
    Task<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>> GetEventTypeAsync(string name);
    Task<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>> CreateEventTypesAsync(string importFilePath);
}