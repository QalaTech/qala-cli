using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class ListEventTypesCommand(IMediator mediator) : AsyncCommand<ListEventTypesArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListEventTypesArgument argument)
    {
        return await mediator.Send(new ListEventTypesRequest())
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[yellow bold]Event Types:[/]");
                    var grid = new Grid()
                        .AddColumns(4)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Type", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Content Type", new Style(decoration: Decoration.Bold))
                        );

                        foreach (var eventType in success.EventTypes)
                        {
                            grid.AddRow(
                                new Text(eventType.Id.ToString()),
                                new Text(eventType.Type),
                                new Text(eventType.Description),
                                new Text(eventType.ContentType)
                            );
                        }
                    AnsiConsole.Write(grid);

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during listing event types:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}
