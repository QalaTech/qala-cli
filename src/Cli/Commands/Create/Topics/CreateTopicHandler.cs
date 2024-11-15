using Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Qala.Cli.Commands.Create.Topics;

public record CreateTopicSuccessResponse(string Name, string Description, Guid[] Events);
public record CreateTopicErrorResponse(string Message);
public record CreateTopicRequest(string Name, string Description, Guid[] Events) : IRequest<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>;

internal class CreateTopicHandler(ITopicService topicService)
    : IRequestHandler<CreateTopicRequest, Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>
{
    public async Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> Handle(CreateTopicRequest request, CancellationToken cancellationToken)
        => await topicService.CreateTopicAsync(request.Name, request.Description, request.Events)
            .ToAsync()
            .Case switch
            {
                CreateTopicSuccessResponse success => success,
                CreateTopicErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}