using Qala.Cli.Utils;
using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Get.EventTypes;

public class EventTypesCommand(IMediator mediator) : AsyncCommand<EventTypesArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, EventTypesArgument eventType)
    {
        if(eventType.Id == null || string.IsNullOrEmpty(eventType.Id))
        {
            return await GetAllAsync(mediator);
        }
        else
        {
            return await GetByIdAsync(mediator, eventType);
        }
    }

    public override ValidationResult Validate(CommandContext context, EventTypesArgument eventType)
    {
        if (!string.IsNullOrEmpty(eventType.Id) && !Guid.TryParse(eventType.Id.ToString(), out _))
        {
            return ValidationResult.Error("The provided Event Type Id is not a valid UUID.");
        }

        return ValidationResult.Success();
    }

    private static async Task<int> GetByIdAsync(IMediator mediator, EventTypesArgument eventType)
    {
        return await mediator.Send(new EventTypeRequest(eventType.Id!))
            .ToAsync()
            .Match(
                success =>
                {
                    BaseCommands.DisplayStart("Get Event Type");
                    AnsiConsole.Write(new Grid()
                        .AddColumns(2)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Type", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.EventType.Id.ToString()),
                            new Text(success.EventType.Type)
                        ));

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error getting event type[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    private static async Task<int> GetAllAsync(IMediator mediator)
    {
        return await mediator.Send(new EventTypesRequest())
            .ToAsync()
            .Match(
                success =>
                {
                    BaseCommands.DisplayStart("Get All Event Types");
                    var grid = new Grid()
                        .AddColumns(2)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Type", new Style(decoration: Decoration.Bold))
                        );

                    foreach (var eventType in success.EventTypes)
                    {
                        grid.AddRow(
                            new Text(eventType.Id.ToString()),
                            new Text(eventType.Type)
                        );
                    }

                    AnsiConsole.Write(grid);

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error getting event types[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}