using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class CreateSubscriptionCommand(IMediator mediator) : AsyncCommand<CreateSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateSubscriptionArgument argument)
    {
        return await mediator.Send(new CreateSubscriptionRequest(argument.TopicName, argument.Name, argument.Description, argument.WebhookUrl, argument.EventTypeIds, argument.MaxDeliveryAttempts))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Subscription created successfully:[/]");
                    AnsiConsole.Write(new Grid()
                        .AddColumns(6)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Name", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                            new Text("Event Types", new Style(decoration: Decoration.Bold)),
                            new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.Subscription.Id.ToString()),
                            new Text(success.Subscription.Name),
                            new Text(success.Subscription.Description),
                            new Text(success.Subscription.WebhookUrl),
                            new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Id.ToString()))),
                            new Text(success.Subscription.MaxDeliveryAttempts.ToString())
                        )
                    );

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during subscription creation:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    public override ValidationResult Validate(CommandContext context, CreateSubscriptionArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.TopicName))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Subscription name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.Description))
        {
            return ValidationResult.Error("Subscription description is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.WebhookUrl))
        {
            return ValidationResult.Error("Webhook URL is required.");
        }

        if (argument.EventTypeIds.Count == 0)
        {
            return ValidationResult.Error("At least one event type id is required.");
        }

        return ValidationResult.Success();
    }
}