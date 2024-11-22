using LanguageExt;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Utils;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class EnvironmentService(IOrganizationService organizationService, ILocalEnvironments localEnvironments) : IEnvironmentService
{
    public Task<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>> CreateEnvironmentAsync(string name, string region, string type)
    {
        throw new NotImplementedException();
    }

    public async Task<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>> GetEnvironmentAsync()
    {
        var environmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);
        if(string.IsNullOrEmpty(environmentId))
        {
            return await Task.FromResult<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>(new GetEnvironmentErrorResponse("No environment was set found"));
        }

        try
        {
            var organization = await organizationService.GetOrganizationAsync();
            if (organization is null)
            {
                return await Task.FromResult<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>(new GetEnvironmentErrorResponse("Organization not found"));
            }

            var environment = organization.Environments.FirstOrDefault(e => e.Id == new Guid(environmentId));
            if(environment is null)
            {
                return await Task.FromResult<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>(new GetEnvironmentErrorResponse("Environment not found"));
            }

            return await Task.FromResult<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>(new GetEnvironemntSuccessResponse(environment));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>(new GetEnvironmentErrorResponse(ex.Message));
        }
    }

    public async Task<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>> SetEnvironmentAsync(Guid environmentId)
    {
        try
        {
            var organization = await organizationService.GetOrganizationAsync();
            if (organization is null)
            {
                return await Task.FromResult<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>(new SetEnvironmentErrorResponse("Organization not found"));
            }

            if(!organization.Environments.Any(e => e.Id == environmentId))
            {
                return await Task.FromResult<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>(new SetEnvironmentErrorResponse("Environment not valid"));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>(new SetEnvironmentErrorResponse(ex.Message));
        }

        localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], environmentId.ToString());
        return await Task.FromResult<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>(new SetEnvironmentSuccessResponse(environmentId));
    }
}