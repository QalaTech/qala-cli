using Qala.Cli.Commands.Config;
using Qala.Cli.Models;
using LanguageExt;

namespace Qala.Cli.Services.Interfaces;

internal interface IConfigService
{
    Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key, string environmentId);
    Task<Config> GetAsync();
}