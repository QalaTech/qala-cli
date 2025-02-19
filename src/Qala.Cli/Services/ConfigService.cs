using Qala.Cli.Commands.Config;
using Qala.Cli.Services.Interfaces;
using LanguageExt;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Utils;

namespace Qala.Cli.Services;

public class ConfigService(ILocalEnvironments localEnvironments) : IConfigService
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key, Guid environmentId)
    {
        if (string.IsNullOrWhiteSpace(key) && environmentId == Guid.Empty)
        {
            return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigErrorResponse("API Key or Environment ID are required"));
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], key);
        }

        if (environmentId != Guid.Empty)
        {
            localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], environmentId.ToString());
        }

        var currentKey = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);
        var currentEnvironmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(currentKey, string.IsNullOrWhiteSpace(currentEnvironmentId) ? Guid.Empty : new Guid(currentEnvironmentId))));
    }

    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> GetAsync()
    {
        var key = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);
        var environmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);

        if (string.IsNullOrEmpty(key))
        {
            return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigErrorResponse("No API key found"));
        }

        if (string.IsNullOrEmpty(environmentId))
        {
            return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigErrorResponse("No environment ID found"));
        }

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(key, new Guid(environmentId))));
    }
}