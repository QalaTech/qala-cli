using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Sources;

public record CreateSourceSuccessResponse(Data.Models.Source Source);
public record CreateSourceErrorResponse(string Message);
public record CreateSourceRequest(string Name, string Description, SourceConfiguration Configuration) : IRequest<Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>>;

public class CreateSourceHandler(ISourceService sourceService)
    : IRequestHandler<CreateSourceRequest, Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>>
{
    public async Task<Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>> Handle(CreateSourceRequest request, CancellationToken cancellationToken)
        => await sourceService.CreateSourceAsync(request.Name, request.Description, request.Configuration)
            .ToAsync()
            .Case switch
        {
            CreateSourceSuccessResponse success => success,
            CreateSourceErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}
