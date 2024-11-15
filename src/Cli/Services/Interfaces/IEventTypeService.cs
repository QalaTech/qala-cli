using Cli.Commands.Get.EventTypes;
using LanguageExt;

namespace Cli.Services.Interfaces;

internal interface IEventTypeService
{
    public Task<Either<EventTypesErrorResponse, EventTypesResponse>> GetAllEventTypesAsync();
    public Task<Either<EventTypeErrorResponse, EventTypeResponse>> GetEventTypeAsync(string id);
}