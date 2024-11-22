using LanguageExt;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class TopicService(ITopicGateway topicGateway) : ITopicService
{
    public async Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> ListTopicsAsync()
    {
        var topics = await topicGateway.ListTopicsAsync();
        return await Task.FromResult<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>(new ListTopicsSuccessResponse(topics));
    }
}
