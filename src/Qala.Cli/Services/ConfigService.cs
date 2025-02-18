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
        localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], key);
        localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], environmentId.ToString());

        var newKey = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);
        var newEnvironmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(newKey, new Guid(newEnvironmentId))));
    }

    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> GetAsync()
    {
        var key = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);
        var environmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);

        if (string.IsNullOrEmpty(key))
        {
            throw new Exception("No API key found");
        }

        if (string.IsNullOrEmpty(environmentId))
        {
            throw new Exception("No environment ID found");
        }

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(new Config(key, new Guid(environmentId))));
    }
}