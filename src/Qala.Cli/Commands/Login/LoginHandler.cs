using LanguageExt;
using MediatR;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Login;

public record LoginSuccessResponse(string Token, List<Data.Models.Environment> Environments);
public record LoginErrorResponse(string Message);
public record LoginRequest() : IRequest<Either<LoginErrorResponse, LoginSuccessResponse>>;

public class LoginHandler(IAuthService authService)
    : IRequestHandler<LoginRequest, Either<LoginErrorResponse, LoginSuccessResponse>>
{
    public async Task<Either<LoginErrorResponse, LoginSuccessResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
        => await authService.LoginAsync()
            .ToAsync()
            .Case switch
            {
                LoginSuccessResponse success => success,
                LoginErrorResponse error => error,
                _ => throw new NotImplementedException()
            };
}