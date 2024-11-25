using LanguageExt;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Gateway.Interfaces;
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

        var topic = await topicGateway.CreateTopicAsync(name, description, eventTypeIds);
        return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicSuccessResponse(topic));
    }

    public async Task<Either<GetTopicErrorResponse, GetTopicSuccessResponse>> GetTopicAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicErrorResponse("Name is required"));
        }
        
        var topic = await topicGateway.GetTopicAsync(name);
        return await Task.FromResult<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>(new GetTopicSuccessResponse(topic));
    }

    public async Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> ListTopicsAsync()
    {
        var topics = await topicGateway.ListTopicsAsync();
        return await Task.FromResult<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>(new ListTopicsSuccessResponse(topics));
    }
}
