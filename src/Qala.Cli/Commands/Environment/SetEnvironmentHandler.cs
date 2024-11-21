using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Environment;

public record SetEnvironmentSuccessResponse(Guid EnvironmentId);
public record SetEnvironmentErrorResponse(string Message);
public record SetEnvironmentRequest(Guid EnvironmentId) : IRequest<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>;

public class SetEnvironmentHandler(IEnvironmentService environmentService)
    : IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>
{
    public async Task<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>> Handle(SetEnvironmentRequest request, CancellationToken cancellationToken)
        => await environmentService.SetEnvironmentAsync(request.EnvironmentId)
            .ToAsync()
            .Case switch
            {
                SetEnvironmentSuccessResponse success => success,
                SetEnvironmentErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}