using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class ListTopicsCommand(IMediator mediator) : AsyncCommand<ListTopicsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListTopicsArgument settings)
    {
        return await mediator.Send(new ListTopicRequest())
            .ToAsync()
            .Match(
                success => 
                {
                    AnsiConsole.MarkupLine("[yellow bold]Topics:[/]");
                    var grid = new Grid()
                        .AddColumns(5)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Name", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                            new Text("Event Types", new Style(decoration: Decoration.Bold))
                        );

                    foreach (var topic in success.Topics)
                    {
                        grid.AddRow(
                            new Text(topic.Id.ToString()),
                            new Text(topic.Name),
                            new Text(topic.Description),
                            new Text(topic.ProvisioningState),
                            new Text(string.Join(", ", topic.EventTypes.Select(et => et.Type)))
                        );
                    }

                    AnsiConsole.Write(grid);

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine("[red bold]Error during listing topics:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}
