using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Topics;

public record UpdateTopicSuccessResponse(Data.Models.Topic Topic);
public record UpdateTopicErrorResponse(string Message);
public record UpdateTopicRequest(string Name, string? NewName, string? Description, List<string>? EventTypeNames) : IRequest<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>;

public class UpdateTopicHandler(ITopicService topicService)
    : IRequestHandler<UpdateTopicRequest, Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>
{
    public async Task<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>> Handle(UpdateTopicRequest request, CancellationToken cancellationToken)
        => await topicService.UpdateTopicAsync(request.Name, request.NewName, request.Description, request.EventTypeNames)
            .ToAsync()
            .Case switch
            {
                UpdateTopicSuccessResponse success => success,
                UpdateTopicErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}    