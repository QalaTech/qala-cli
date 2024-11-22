using Qala.Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Qala.Cli.Commands.Config;

public record ConfigSuccessResponse(Data.Models.Config Config);
public record ConfigErrorResponse(string Message);
public record ConfigRequest(string Key, Guid EnvironmentId) : IRequest<Either<ConfigErrorResponse, ConfigSuccessResponse>>;

public class ConfigHandler(IConfigService configService)
    : IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> Handle(ConfigRequest request, CancellationToken cancellationToken)
        => await configService.CreateConfigAsync(request.Key, request.EnvironmentId)
            .ToAsync()
            .Case switch
            {
                ConfigSuccessResponse success => success,
                ConfigErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}