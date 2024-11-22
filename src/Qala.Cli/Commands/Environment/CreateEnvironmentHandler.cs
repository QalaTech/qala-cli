using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Environment;

public record CreateEnvironmentSuccessResponse(Data.Models.Environment Environment);
public record CreateEnvironmentErrorResponse(string Message);
public record CreateEnvironmentRequest(string Name, string Region, string Type) : IRequest<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>;

public class CreateEnvironmentHandler(IEnvironmentService environmentService)
    : IRequestHandler<CreateEnvironmentRequest, Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>
{
    public async Task<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>> Handle(CreateEnvironmentRequest request, CancellationToken cancellationToken)
        => await environmentService.CreateEnvironmentAsync(request.Name, request.Region, request.Type)
            .ToAsync()
            .Case switch
            {
                CreateEnvironmentSuccessResponse success => success,
                CreateEnvironmentErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}