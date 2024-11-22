using LanguageExt;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Utils;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class EnvironmentService(
    IEnvironmentGateway environmentGateway,
    IOrganizationGateway organizationService, 
    ILocalEnvironments localEnvironments) : IEnvironmentService
{
    public async Task<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>> CreateEnvironmentAsync(string name, string region, string type)
    {
        if(string.IsNullOrEmpty(name))
        {
            return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentErrorResponse("Name is required"));
        }

        if(string.IsNullOrEmpty(region))
        {
            return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentErrorResponse("Region is required"));
        }

        if(string.IsNullOrEmpty(type))
        {
            return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentErrorResponse("Type is required"));
        }

        try
        {
            var newEnvironment = new Data.Models.Environment
            {
                Name = name,
                Region = region,
                EnvironmentType = type
            };

            var environmentCreated = await environmentGateway.CreateEnvironmentAsync(newEnvironment);

            if(environmentCreated is null)
            {
                return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentErrorResponse("Failed to create environment"));
            }

            return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentSuccessResponse(environmentCreated));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>(new CreateEnvironmentErrorResponse(ex.Message));
        }
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