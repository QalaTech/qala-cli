using Cli.Commands.Config;
using Cli.Models;
using LanguageExt;

namespace Cli.Services.Interfaces;

internal interface IConfigService
{
    Task<Either<CreateConfigErrorResponse, CreateConfigSuccessResponse>> CreateConfigAsync(string key, string environmentId);
    Task<Config> GetAsync();
}