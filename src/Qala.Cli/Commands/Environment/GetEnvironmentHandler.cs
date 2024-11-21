using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Environment;

public record GetEnvironemntSuccessResponse(Models.Environment Environment);
public record GetEnvironmentErrorResponse(string Message);
public record GetEnvironmentRequest() : IRequest<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>;

public class GetEnvironmentHandler(IEnvironmentService environmentService)
    : IRequestHandler<GetEnvironmentRequest, Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>
{
    public async Task<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>> Handle(GetEnvironmentRequest request, CancellationToken cancellationToken)
        => await environmentService.GetEnvironmentAsync()
            .ToAsync()
            .Case switch
            {
                GetEnvironemntSuccessResponse success => success,
                GetEnvironmentErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}