using LanguageExt;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class TopicService(ITopicGateway topicGateway, IEventTypeGateway eventTypeGateway) : ITopicService
{
    public async Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> CreateTopicAsync(string name, string description, List<string> eventTypeNames)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Name is required"));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Description is required"));
        }

        if (eventTypeNames == null || eventTypeNames.Count == 0)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Event type names are required"));
        }

        var eventTypes = await eventTypeGateway.ListEventTypesAsync();

        if (eventTypes == null || !eventTypes.Any())
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Event types not found"));
        }

        List<Guid> eventTypeIds = eventTypes
            .Where(e => e?.Type != null && eventTypeNames.Contains(e.Type))
            .Select(e => e?.Id)
            .Where(id => id.HasValue)
            .Select(id => id.GetValueOrDefault())
            .ToList();

        if (eventTypeIds == null || eventTypeIds.Count == 0)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Event types not found"));
        }

        var topic = await topicGateway.CreateTopicAsync(name, description, eventTypeIds);
        if (topic == null)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Failed to create topic"));
        }

        return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicSuccessResponse(topic));
    }

    public async Task<Either<GetTopicErrorResponse, GetTopicSuccessResponse>> GetTopicAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse("Name is required"));
        }

        var topic = await topicGateway.GetTopicAsync(name);
        if (topic == null)
        {
            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse("Topic not found"));
        }

        return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicSuccessResponse(topic));
    }

    public async Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> ListTopicsAsync()
    {
        var topics = await topicGateway.ListTopicsAsync();
        return await Task.FromResult<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>(new ListTopicsSuccessResponse(topics));
    }

    public async Task<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>> UpdateTopicAsync(string name, string? description, List<string>? eventTypeNames)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse("Name is required"));
        }

        var topic = await topicGateway.GetTopicAsync(name);

        if (topic == null)
        {
            return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse("Topic not found"));
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            topic.Description = description;
        }

        if (eventTypeNames != null && eventTypeNames.Count != 0)
        {
            var eventTypes = await eventTypeGateway.ListEventTypesAsync();
            if (eventTypes == null || !eventTypes.Any())
            {
                return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse("Event types not found"));
            }

            if (topic.EventTypes.Select(e => e.Type).ToList() != eventTypeNames)
            {
                var newEventTypeIds = eventTypes
                    .Where(e => e?.Type != null && eventTypeNames.Contains(e.Type))
                    .Select(e => e?.Id)
                    .Where(id => id.HasValue)
                    .Select(id => id.GetValueOrDefault())
                    .ToList();

                topic.EventTypes = newEventTypeIds.Select(id => new EventType { Id = id }).ToList();
            }
        }

        var updatedTopic = await topicGateway.UpdateTopicAsync(
            name,
            topic.Description,
            topic.EventTypes.Select(et => et.Id).ToList());
        if (updatedTopic == null)
        {
            return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse("Failed to update topic"));
        }

        return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicSuccessResponse(updatedTopic));
    }
}
