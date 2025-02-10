using LanguageExt;
using Qala.Cli.Commands.Environment;

namespace Qala.Cli.Services.Interfaces;

public interface IEnvironmentService
{
    Task<Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>> CreateEnvironmentAsync(string name, string region, string type, bool disableSchemaValidation);
    Task<Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>> GetEnvironmentAsync();
    Task<Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>> SetEnvironmentAsync(Guid environmentId);
    Task<Either<UpdateEnvironmentErrorResponse, UpdateEnvironmentSuccessResponse>> UpdateEnvironmentAsync(bool disableSchemaValidation);
}