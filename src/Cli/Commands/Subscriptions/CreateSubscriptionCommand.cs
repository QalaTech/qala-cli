using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands.Subscriptions;

public class CreateSubscriptionCommand(IMediator mediator) : AsyncCommand<CreateSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateSubscriptionArgument settings)
        => await mediator.Send(new CreateSubscriptionRequest(settings.Name, settings.TopicName, settings.Description, settings.EventTypeIds, settings.WebhookUrl, settings.MaxDeliveryAttempts))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.Write(
                        new FigletText("Qala CLI")
                            .LeftJustified()
                            .Color(Color.Yellow1));
                    AnsiConsole.MarkupLine($"[yellow bold]Creating subscription[/]");

                    AnsiConsole.Write(new Grid()
                        .AddColumns(6)
                        .AddRow(
                            new Text("Name", new Style(decoration: Decoration.Bold)),
                            new Text("Topic", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Events", new Style(decoration: Decoration.Bold)),
                            new Text("Webhook URL", new Style(decoration: Decoration.Bold)),
                            new Text("Max Retries", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.Name), 
                            new Text(success.TopicName), 
                            new Text(success.Description), 
                            new Text(string.Join(", ", success.EventTypeIds)), 
                            new Text(success.WebhookUrl), 
                            new Text(success.MaxDeliveryAttempts.ToString()))
                    );

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error creating subscription[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
}