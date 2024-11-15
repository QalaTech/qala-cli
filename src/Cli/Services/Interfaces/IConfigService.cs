using Cli.Commands.Config;
using Cli.Models;
using LanguageExt;

namespace Qala.Cli.Services.Interfaces;

internal interface IConfigService
{
    Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key);
    Task<Config> GetAsync();
}