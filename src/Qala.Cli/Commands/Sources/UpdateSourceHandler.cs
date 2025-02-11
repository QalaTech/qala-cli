using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Sources;

public record UpdateSourceSuccessResponse(Data.Models.Source Source);
public record UpdateSourceErrorResponse(string Message);
public record UpdateSourceRequest(string SourceName, string Name, string Description, SourceConfiguration Configuration) : IRequest<Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>>;

public class UpdateSourceHandler(ISourceService sourceService)
    : IRequestHandler<UpdateSourceRequest, Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>>
{
    public async Task<Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>> Handle(UpdateSourceRequest request, CancellationToken cancellationToken)
        => await sourceService.UpdateSourceAsync(request.SourceName, request.Name, request.Description, request.Configuration)
            .ToAsync()
            .Case switch
        {
            UpdateSourceSuccessResponse success => success,
            UpdateSourceErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}