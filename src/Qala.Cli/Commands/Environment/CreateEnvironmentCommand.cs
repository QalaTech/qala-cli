using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class CreateEnvironmentCommand(IMediator mediator) : AsyncCommand<CreateEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateEnvironmentArgument argument)
    {
        return await mediator.Send(new CreateEnvironmentRequest(argument.Name, argument.Region, argument.Type))
        .ToAsync()
        .Match(
            success => 
            {
                AnsiConsole.MarkupLine($"[green bold]Environment created successfully[/]");
                
                return 0;
            },
            error => 
            {
                AnsiConsole.MarkupLine($"[red bold]Error creating environment[/]");
                AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                return -1;
            }
        );
    }
}