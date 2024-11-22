using LanguageExt;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class EventTypeService(IEventTypeGateway eventTypeGateway) : IEventTypeService
{
    public async Task<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>> GetEventTypeAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeErrorResponse("Invalid id"));
        }
        
        var eventType = await eventTypeGateway.GetEventTypeAsync(id);
        return await Task.FromResult<Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>(new GetEventTypeSuccessResponse(eventType));
    }

    public async Task<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>> ListEventTypesAsync()
    {
        var eventTypes = await eventTypeGateway.ListEventTypesAsync();
        return await Task.FromResult<Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>(new ListEventTypesSuccessResponse(eventTypes.ToArray()));
    }
}
