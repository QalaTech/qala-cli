using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class CreateSubscriptionCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateSubscriptionArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new CreateSubscriptionRequest(argument.TopicName, argument.SourceName, argument.Name, argument.Description, argument.WebhookUrl, argument.EventTypeNames, argument.MaxDeliveryAttempts))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscription", BaseCommands.CommandAction.Create, console);
                            console.Write(new Grid()
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
                                    new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Type))),
                                    new Text(success.Subscription.MaxDeliveryAttempts.ToString())
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscription", BaseCommands.CommandAction.Create, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateSubscriptionArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.TopicName) && string.IsNullOrWhiteSpace(argument.SourceName))
        {
            return ValidationResult.Error("Either Topic name or Source name must be provided.");
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

        if (argument.EventTypeNames.Count == 0)
        {
            return ValidationResult.Error("At least one event type name is required.");
        }

        return ValidationResult.Success();
    }
}