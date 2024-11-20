using Qala.Cli.Commands.Config;
using Qala.Cli.Models;
using Qala.Cli.Services.Interfaces;
using LanguageExt;
using Qala.Cli.Utils;

namespace Qala.Cli.Services;

internal class ConfigService() : IConfigService
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key, string environmentId)
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], key, EnvironmentVariableTarget.User);
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], environmentId, EnvironmentVariableTarget.User);

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(key));
    }

    public async Task<Config> GetAsync()
    {
        var apiKey = Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);
        var environmentId = Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User);

        if(string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("No API key found");
        }

        if(string.IsNullOrEmpty(environmentId))
        {
            throw new Exception("No environment ID found");
        }

        return await Task.FromResult(new Config(apiKey, environmentId));
    }
}