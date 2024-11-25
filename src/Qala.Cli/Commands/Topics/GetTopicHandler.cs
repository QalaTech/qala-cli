using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Topics;

public record GetTopicSuccessResponse(Data.Models.Topic Topic);
public record GetTopicErrorResponse(string Message);
public record GetTopicRequest(string Name) : IRequest<Either<GetTopicErrorResponse, GetTopicSuccessResponse>>;

public class GetTopicHandler(ITopicService topicService)
    : IRequestHandler<GetTopicRequest, Either<GetTopicErrorResponse, GetTopicSuccessResponse>>
{
    public async Task<Either<GetTopicErrorResponse, GetTopicSuccessResponse>> Handle(GetTopicRequest request, CancellationToken cancellationToken)
        => await topicService.GetTopicAsync(request.Name)
            .ToAsync()
            .Case switch
            {
                GetTopicSuccessResponse success => success,
                GetTopicErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}