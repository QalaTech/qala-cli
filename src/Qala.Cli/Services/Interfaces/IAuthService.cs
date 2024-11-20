using LanguageExt;
using Qala.Cli.Commands.Login;

namespace Qala.Cli.Services.Interfaces;

public interface IAuthService
{
    Task<Either<LoginErrorResponse, LoginSuccessResponse>> LoginAsync();
}