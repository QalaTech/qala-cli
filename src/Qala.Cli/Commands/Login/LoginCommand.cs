using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Login;

public class LoginCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<LoginArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, LoginArgument argument)
    {
        BaseCommands.DisplayLogoStart("Login", console);
        return await mediator.Send(new LoginRequest())
            .ToAsync()
            .Match(
                success => 
                {
                    var environmentName = console.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What's the [green]environment[/] that you want to work on?")
                            .PageSize(10)
                            .MoreChoicesText("[grey](Move up and down to reveal more environments)[/]")
                            .AddChoices(success.Environments.Select(e => e.Name).ToArray()));

                    var environmentId = success.Environments.First(e => e.Name == environmentName).Id.ToString();
                    
                    System.Environment.SetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], environmentId, EnvironmentVariableTarget.User);
                    console.MarkupLine($"[green bold]Login Successful[/]");

                    return 0;
                },
                error =>
                {
                    console.MarkupLine($"[red bold]Error logging in[/]");
                    console.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}