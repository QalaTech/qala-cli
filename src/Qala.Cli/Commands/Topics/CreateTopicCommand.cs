using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class CreateTopicCommand(IMediator mediator) : AsyncCommand<CreateTopicArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateTopicArgument argument)
    {
        return await mediator.Send(new CreateTopicRequest(argument.Name, argument.Description, argument.EventTypeIds))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Topic created successfully:[/]");
                    AnsiConsole.Write(new Grid()
                        .AddColumns(5)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Name", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                            new Text("Event Types", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.Topic.Id.ToString()),
                            new Text(success.Topic.Name),
                            new Text(success.Topic.Description),
                            new Text(success.Topic.ProvisioningState),
                            new Text(string.Join(", ", success.Topic.EventTypes.Select(et => et.Id.ToString())))
                        )
                    );

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during topic creation:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    public override ValidationResult Validate(CommandContext context, CreateTopicArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.Description))
        {
            return ValidationResult.Error("Topic description is required.");
        }

        if (argument.EventTypeIds.Count == 0)
        {
            return ValidationResult.Error("At least one event type id is required.");
        }

        return ValidationResult.Success();
    }
}