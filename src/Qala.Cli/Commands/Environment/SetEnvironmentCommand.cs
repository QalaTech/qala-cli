using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class SetEnvironmentCommand(IMediator mediator) : AsyncCommand<SetEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SetEnvironmentArgument argument)
    {
        return await mediator.Send(new SetEnvironmentRequest(argument.EnvironmentId))
        .ToAsync()
        .Match(
            success => 
            {
                AnsiConsole.MarkupLine($"[green bold]Environment set successfully[/]");
                
                return 0;
            },
            error => 
            {
                AnsiConsole.MarkupLine($"[red bold]Error setting environment[/]");
                AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                return -1;
            }
        );
    }
}