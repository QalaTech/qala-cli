using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Topics;

public record ListTopicsSuccessResponse(IEnumerable<Topic> Topics);
public record ListTopicsErrorResponse(string Message);
public record ListTopicRequest : IRequest<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>;

public class ListTopicsHandler(ITopicService topicService)
    : IRequestHandler<ListTopicRequest, Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>
{
    public async Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> Handle(ListTopicRequest request, CancellationToken cancellationToken)
        => await topicService.ListTopicsAsync()
            .ToAsync()
            .Case switch
            {
                ListTopicsSuccessResponse success => success,
                ListTopicsErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}