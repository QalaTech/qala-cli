using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Environment;

public record UpdateEnvironmentSuccessResponse(Data.Models.Environment Environment);
public record UpdateEnvironmentErrorResponse(string Message);
public record UpdateEnvironmentRequest(bool DisableSchemaValidation) : IRequest<Either<UpdateEnvironmentErrorResponse, UpdateEnvironmentSuccessResponse>>;

public class UpdateEnvironmentHandler(IEnvironmentService environmentService)
    : IRequestHandler<UpdateEnvironmentRequest, Either<UpdateEnvironmentErrorResponse, UpdateEnvironmentSuccessResponse>>
{
    public async Task<Either<UpdateEnvironmentErrorResponse, UpdateEnvironmentSuccessResponse>> Handle(UpdateEnvironmentRequest request, CancellationToken cancellationToken)
        => await environmentService.UpdateEnvironmentAsync(request.DisableSchemaValidation)
            .ToAsync()
            .Case switch
        {
            UpdateEnvironmentSuccessResponse success => success,
            UpdateEnvironmentErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}