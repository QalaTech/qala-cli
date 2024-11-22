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
                AnsiConsole.Write(new Grid()
                    .AddColumns(4)
                    .AddRow(
                        new Text("ID", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Region", new Style(decoration: Decoration.Bold)),
                        new Text("Type", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text(success.Environment.Id.ToString()),
                        new Text(success.Environment.Name),
                        new Text(success.Environment.Region),
                        new Text(success.Environment.EnvironmentType)
                    )
                );
                
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