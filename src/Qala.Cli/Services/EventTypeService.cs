using LanguageExt;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class EventTypeService(IEventTypeGateway eventTypeGateway) : IEventTypeService
{
    public async Task<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>> GetEventTypeAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse("Invalid name"));
        }
        
        try
        {
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
        catch (Exception e)
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse(e.Message));
        }
    }

    public async Task<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>> ListEventTypesAsync()
    {
        try
        {
            var eventTypes = await eventTypeGateway.ListEventTypesAsync();
            return await Task.FromResult<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>(new ListEventTypesSuccessResponse(eventTypes));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>(new ListEventTypesErrorResponse(e.Message));
        }
    }
}
