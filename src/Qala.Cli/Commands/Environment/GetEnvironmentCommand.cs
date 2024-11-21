using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class GetEnvironmentCommand(IMediator mediator) : AsyncCommand<GetEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetEnvironmentArgument argument)
    {
        return await mediator.Send(new GetEnvironmentRequest())
        .ToAsync()
        .Match(
            success => 
            {
                AnsiConsole.MarkupLine($"[green bold]Environment found successfully[/]");
                
                return 0;
            },
            error => 
            {
                AnsiConsole.MarkupLine($"[red bold]Error finding environment[/]");
                AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                return -1;
            }
        );
    }
}