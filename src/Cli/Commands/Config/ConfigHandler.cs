using Qala.Cli.Services.Interfaces;
using LanguageExt;
using MediatR;

namespace Qala.Cli.Commands.Config;

public record ConfigSuccessResponse(string Key);
public record ConfigErrorResponse(string Message);
public record ConfigRequest(string Key) : IRequest<Either<ConfigErrorResponse, ConfigSuccessResponse>>;

internal class CreateConfigHandler(IConfigService configService)
    : IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> Handle(ConfigRequest request, CancellationToken cancellationToken)
        => await configService.CreateConfigAsync(request.Key)
            .ToAsync()
            .Case switch
            {
                ConfigSuccessResponse success => success,
                ConfigErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}