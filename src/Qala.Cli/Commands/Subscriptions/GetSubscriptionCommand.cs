using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetSubscriptionCommand(IMediator mediator) : AsyncCommand<GetSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetSubscriptionArgument arguments)
    {
        return await mediator.Send(new GetSubscriptionRequest(arguments.TopicName, arguments.SubscriptionId))
            .ToAsync()
            .Match(
                success => 
                {
                    AnsiConsole.MarkupLine("[yellow bold]Subscription:[/]");
                    var grid = new Grid()
                        .AddColumns(8)
                        .AddRow(
                            new Text("Id", new Style(decoration: Decoration.Bold)),
                            new Text("Name", new Style(decoration: Decoration.Bold)),
                            new Text("Description", new Style(decoration: Decoration.Bold)),
                            new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                            new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                            new Text("Event Types", new Style(decoration: Decoration.Bold)),
                            new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                            new Text("Deadletters count", new Style(decoration: Decoration.Bold))
                        );

                    grid.AddRow(
                        new Text(success.Subscription.Id.ToString()),
                        new Text(success.Subscription.Name),
                        new Text(success.Subscription.Description),
                        new Text(success.Subscription.WebhookUrl),
                        new Text(success.Subscription.ProvisioningState),
                        new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Type))),
                        new Text(success.Subscription.MaxDeliveryAttempts.ToString()),
                        new Text(success.Subscription.DeadletterCount.ToString())
                    );

                    AnsiConsole.Write(grid);

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine("[red bold]Error during getting subscription:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    public override ValidationResult Validate(CommandContext context, GetSubscriptionArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.TopicName))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (arguments.SubscriptionId == Guid.Empty)
        {
            return ValidationResult.Error("Subscription id is required.");
        }

        return ValidationResult.Success();
    }
}