using Qala.Cli.Commands.Config;
using Qala.Cli.Services.Interfaces;
using LanguageExt;
using Qala.Cli.Utils;
using Qala.Cli.Models;

namespace Qala.Cli.Services;

public class ConfigService() : IConfigService
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key, Guid environmentId)
    {
        System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], key, EnvironmentVariableTarget.User);
        System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], environmentId.ToString(), EnvironmentVariableTarget.User);

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(key, environmentId)));
    }

    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> GetAsync()
    {
        var key = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);
        var environmentId = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User);

        if(string.IsNullOrEmpty(key))
        {
            throw new Exception("No API key found");
        }

        if(string.IsNullOrEmpty(environmentId))
        {
            throw new Exception("No environment ID found");
        }

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(key, new Guid(environmentId))));
    }
}