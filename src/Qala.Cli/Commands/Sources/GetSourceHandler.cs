using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Sources;

public record GetSourceSuccessResponse(Data.Models.Source Source);
public record GetSourceErrorResponse(string Message);
public record GetSourceRequest(string Name) : IRequest<Either<GetSourceErrorResponse, GetSourceSuccessResponse>>;

public class GetSourceHandler(ISourceService sourceService)
    : IRequestHandler<GetSourceRequest, Either<GetSourceErrorResponse, GetSourceSuccessResponse>>
{
    public async Task<Either<GetSourceErrorResponse, GetSourceSuccessResponse>> Handle(GetSourceRequest request, CancellationToken cancellationToken)
        => await sourceService.GetSourceAsync(request.Name)
            .ToAsync()
            .Case switch
        {
            GetSourceSuccessResponse success => success,
            GetSourceErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}