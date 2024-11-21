using Qala.Cli.Commands.Config;
using LanguageExt;

namespace Qala.Cli.Services.Interfaces;

public interface IConfigService
{
    Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key, Guid environmentId);
    Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> GetAsync();
}