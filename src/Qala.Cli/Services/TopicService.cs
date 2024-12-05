using LanguageExt;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class TopicService(ITopicGateway topicGateway) : ITopicService
{
    public async Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> CreateTopicAsync(string name, string description, List<Guid> eventTypeIds)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Name is required"));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Description is required"));
        }

        if (eventTypeIds == null || eventTypeIds.Count == 0)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Event type ids are required"));
        }

        try
        {
            var topic = await topicGateway.CreateTopicAsync(name, description, eventTypeIds);
            if (topic == null)
            {
                return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Failed to create topic"));
            }

            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicSuccessResponse(topic));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse(e.Message));
        }

        
    }

    public async Task<Either<GetTopicErrorResponse, GetTopicSuccessResponse>> GetTopicAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse("Name is required"));
        }
        
        try
        {
            var topic = await topicGateway.GetTopicAsync(name);
            if (topic == null)
            {
                return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse("Topic not found"));
            }

            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicSuccessResponse(topic));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse(e.Message));
        }
    }

    public async Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> ListTopicsAsync()
    {
        try
        {
            var topics = await topicGateway.ListTopicsAsync();
            return await Task.FromResult<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>(new ListTopicsSuccessResponse(topics));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>(new ListTopicsErrorResponse(e.Message));
        }
    }

    public async Task<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>> UpdateTopicAsync(string name, string? newName, string? description, List<Guid>? eventTypeIds)
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

        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = topic.Name;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            topic.Description = description;
        }

        if (eventTypeIds != null && eventTypeIds.Count > 0 &&
            topic.EventTypes.Select(e => e.Id).ToList() != eventTypeIds
         )
        {
            topic.EventTypes = eventTypeIds.Select(id => new EventType { Id = id }).ToList();
        }

        try
        {
            var updatedTopic = await topicGateway.UpdateTopicAsync(
                topic.Name, 
                newName,
                topic.Description, 
                topic.EventTypes.Select(e => e.Id).ToList());
            if (updatedTopic == null)
            {
                return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse("Failed to update topic"));
            }

            return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicSuccessResponse(updatedTopic));
        }
        catch (Exception e)
        {
            return await Task.FromResult<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>(new UpdateTopicErrorResponse(e.Message));
        }
    }
}
