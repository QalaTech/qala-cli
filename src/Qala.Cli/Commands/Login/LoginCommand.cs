using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Login;

public class LoginCommand(IMediator mediator) : AsyncCommand<LoginArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, LoginArgument login)
    {
        BaseCommands.DisplayStart("Login");
        return await mediator.Send(new LoginRequest())
            .ToAsync()
            .Match(
                success => 
                {
                    AnsiConsole.MarkupLine($"[green bold]Login Successful[/]");

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error logging in[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}