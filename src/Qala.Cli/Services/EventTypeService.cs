using LanguageExt;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class EventTypeService(IEventTypeGateway eventTypeGateway) : IEventTypeService
{
    public async Task<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>> CreateEventTypesAsync(string importFilePath)
    {
        if (string.IsNullOrWhiteSpace(importFilePath))
        {
            return await Task.FromResult<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>>(new CreateEventTypesErrorResponse("Import file path is required"));
        }

        var numberOfEventTypesImported = await eventTypeGateway.ImportOpenApiSpecAsync(importFilePath);

        if (numberOfEventTypesImported == 0)
        {
            return await Task.FromResult<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>>(new CreateEventTypesErrorResponse("No event types imported"));
        }

        var eventTypes = await eventTypeGateway.ListEventTypesAsync();
        return await Task.FromResult<Either<CreateEventTypesErrorResponse, CreateEventTypesSuccessResponse>>(new CreateEventTypesSuccessResponse([.. eventTypes]));
    }

    public async Task<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>> GetEventTypeAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse("Event Type name is required"));
        }

        var eventTypes = await eventTypeGateway.ListEventTypesAsync();
        if (eventTypes == null || !eventTypes.Any())
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse("Event type not found"));
        }

        var eventType = eventTypes?.FirstOrDefault(x => x?.Type == name) ?? null;

        if (eventType == null)
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse("Event type not found"));
        }

        return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeSuccessResponse(eventType));
    }

    public async Task<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>> ListEventTypesAsync()
    {
        var eventTypes = await eventTypeGateway.ListEventTypesAsync();
        return await Task.FromResult<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>(new ListEventTypesSuccessResponse(eventTypes));
    }
}
