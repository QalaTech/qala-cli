using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Topics;

public record CreateTopicSuccessResponse(Data.Models.Topic Topic);
public record CreateTopicErrorResponse(string Message);
public record CreateTopicRequest(string Name, string Description, List<Guid> EventTypeIds) : IRequest<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>;

public class CreateTopicHandler(ITopicService topicService)
    : IRequestHandler<CreateTopicRequest, Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>
{
    public async Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> Handle(CreateTopicRequest request, CancellationToken cancellationToken)
        => await topicService.CreateTopicAsync(request.Name, request.Description, request.EventTypeIds)
            .ToAsync()
            .Case switch
            {
                CreateTopicSuccessResponse success => success,
                CreateTopicErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}