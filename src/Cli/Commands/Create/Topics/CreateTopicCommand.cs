using Cli.Utils;
using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Create.Topics;

public class TopicsCommand(IMediator mediator) : AsyncCommand<TopicsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, TopicsArgument settings)
        => await mediator.Send(new CreateTopicRequest(settings.Name, settings.Description, settings.Events))
            .ToAsync()
            .Match(
                success => {
                    
                    BaseCommands.DisplayStart("Create Topic");

                    AnsiConsole.Write(new Grid()
                        .AddColumns(3)
                        .AddRow(
                            new Text("Name", new Style(decoration: Decoration.Bold)), 
                            new Text("Description", new Style(decoration: Decoration.Bold)), 
                            new Text("Events", new Style(decoration: Decoration.Bold)))
                        .AddRow(
                            new Text(success.Name), 
                            new Text(success.Description), 
                            new Text(string.Join(", ", success.Events)))
                    );

                    return 0;
                },
                error => {
                    AnsiConsole.MarkupLine($"[red bold]Error creating topic[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    
    public override ValidationResult Validate(CommandContext context, TopicsArgument settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            return ValidationResult.Error("Name is required");
        }

        if (string.IsNullOrWhiteSpace(settings.Description))
        {
            return ValidationResult.Error("Description is required");
        }

        if (settings.Events is null || settings.Events.Length == 0)
        {
            return ValidationResult.Error("Events are required");
        }

        return ValidationResult.Success();
    }
}