using Cli.Utils;
using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Create.Subscriptions;

public class SubscriptionsCommand(IMediator mediator) : AsyncCommand<SubscriptionsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SubscriptionsArgument settings)
        => await mediator.Send(new CreateSubscriptionRequest(settings.Name, settings.TopicName, settings.Description, settings.EventTypeIds, settings.WebhookUrl, settings.MaxDeliveryAttempts))
            .ToAsync()
            .Match(
                success =>
                {
                    BaseCommands.DisplayStart("Create Subscription");

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
    public override ValidationResult Validate(CommandContext context, SubscriptionsArgument settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            return ValidationResult.Error("Name is required");
        }

        if (string.IsNullOrWhiteSpace(settings.TopicName))
        {
            return ValidationResult.Error("Topic is required");
        }

        if (string.IsNullOrWhiteSpace(settings.Description))
        {
            return ValidationResult.Error("Description is required");
        }

        if (settings.EventTypeIds is null || settings.EventTypeIds.Length == 0)
        {
            return ValidationResult.Error("Events are required");
        }

        if (string.IsNullOrWhiteSpace(settings.WebhookUrl))
        {
            return ValidationResult.Error("Webhook URL is required");
        }

        if (settings.MaxDeliveryAttempts < 0 && settings.MaxDeliveryAttempts > 10)
        {
            return ValidationResult.Error("Max Retries must be between 0 and 10");
        }

        return ValidationResult.Success();
    }
}