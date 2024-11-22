using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class GetEventTypeCommand(IMediator mediator) : AsyncCommand<GetEventTypeArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetEventTypeArgument argument)
    {
        return await mediator.Send(new GetEventTypeRequest(argument.Id))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Event type retrieved:[/]");
                    AnsiConsole.Write(new Grid()
                        .AddColumns(5)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Type", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Content Type", new Style(decoration: Decoration.Bold)),
                            new Text("Categories", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.EventType.Id.ToString()),
                            new Text(success.EventType.Type.ToString()),
                            new Text(success.EventType.Description),
                            new Text(success.EventType.ContentType),
                            new Text(string.Join(", ", success.EventType.Categories))
                        )
                    );
                    
                    if(!string.IsNullOrEmpty(success.EventType.Schema))
                    {
                        AnsiConsole.Write(
                            new Panel(success.EventType.Schema)
                                .Header("[bold]Schema[/]")
                                .RoundedBorder()
                                .BorderColor(Color.Blue)
                        );    
                    }                        

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during event type retrieval:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    public override ValidationResult Validate(CommandContext context, GetEventTypeArgument argument)
    {
        if (argument.Id == Guid.Empty)
        {
            return ValidationResult.Error("Event type ID is required");
        }

        return ValidationResult.Success();
    }
}
