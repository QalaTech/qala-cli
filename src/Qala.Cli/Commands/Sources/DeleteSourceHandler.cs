using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Sources;

public record DeleteSourceSuccessResponse();
public record DeleteSourceErrorResponse(string Message);
public record DeleteSourceRequest(string Name) : IRequest<Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>>;

public class DeleteSourceHandler(ISourceService sourceService)
    : IRequestHandler<DeleteSourceRequest, Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>>
{
    public async Task<Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>> Handle(DeleteSourceRequest request, CancellationToken cancellationToken)
        => await sourceService.DeleteSourceAsync(request.Name)
            .ToAsync()
            .Case switch
        {
            DeleteSourceSuccessResponse success => success,
            DeleteSourceErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}