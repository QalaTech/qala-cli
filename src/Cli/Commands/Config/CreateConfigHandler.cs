using Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Cli.Commands.Config;

public record CreateConfigSuccessResponse(string Key, string EnvironmentId);
public record CreateConfigErrorResponse(string Message);
public record CreateConfigRequest(string Key, string EnvironmentId) : IRequest<Either<CreateConfigErrorResponse, CreateConfigSuccessResponse>>;

internal class CreateConfigHandler(IConfigService configService)
    : IRequestHandler<CreateConfigRequest, Either<CreateConfigErrorResponse, CreateConfigSuccessResponse>>
{
    public async Task<Either<CreateConfigErrorResponse, CreateConfigSuccessResponse>> Handle(CreateConfigRequest request, CancellationToken cancellationToken)
        => await configService.CreateConfigAsync(request.Key, request.EnvironmentId)
            .ToAsync()
            .Case switch
            {
                CreateConfigSuccessResponse success => success,
                CreateConfigErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}