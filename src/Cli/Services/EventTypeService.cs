using Cli.Models;
using Cli.Services.Interfaces;
using LanguageExt;
using Cli.Commands.Get.EventTypes;

namespace Cli.Services;

internal class EventTypeService(HttpClient publishingApiClient) : IEventTypeService
{
    public async Task<Either<EventTypesErrorResponse, EventTypesResponse>> GetAllEventTypesAsync()
    {
        return await Task.FromResult<Either<EventTypesErrorResponse, EventTypesResponse>>(
            new EventTypesResponse(
                [
                    new EventType(new Guid(), "Type 1"),
                    new EventType(new Guid(), "Type 2"),
                    new EventType(new Guid(), "Type 3")
                ]
            )
        );
    }

    public async Task<Either<EventTypeErrorResponse, EventTypeResponse>> GetEventTypeAsync(string id)
    {
        return await Task.FromResult<Either<EventTypeErrorResponse, EventTypeResponse>>(
            new EventTypeResponse(new EventType(Guid.Parse(id), "Type 1"))
        );
    }
}