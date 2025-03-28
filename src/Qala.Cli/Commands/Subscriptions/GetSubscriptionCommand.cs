using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetSubscriptionCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetSubscriptionArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new GetSubscriptionRequest(arguments.TopicName, arguments.SourceName, arguments.SubscriptionName))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscription", BaseCommands.CommandAction.Get, console);
                            var grid = string.IsNullOrWhiteSpace(success.Subscription.Audience) ?
                                new Grid()
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
                                    )
                                    .AddRow(
                                        new Text(success.Subscription.Id.ToString()),
                                        new Text(success.Subscription.Name),
                                        new Text(success.Subscription.Description),
                                        new Text(success.Subscription.WebhookUrl),
                                        new Text(success.Subscription.ProvisioningState),
                                        new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Type))),
                                        new Text(success.Subscription.MaxDeliveryAttempts.ToString()),
                                        new Text(success.Subscription.DeadletterCount.ToString())
                                    ) :
                                new Grid()
                                    .AddColumns(9)
                                    .AddRow(
                                        new Text("Id", new Style(decoration: Decoration.Bold)),
                                        new Text("Name", new Style(decoration: Decoration.Bold)),
                                        new Text("Description", new Style(decoration: Decoration.Bold)),
                                        new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                                        new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                        new Text("Event Types", new Style(decoration: Decoration.Bold)),
                                        new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                                        new Text("Deadletters count", new Style(decoration: Decoration.Bold)),
                                        new Text("Audience", new Style(decoration: Decoration.Bold))
                                    )
                                    .AddRow(
                                        new Text(success.Subscription.Id.ToString()),
                                        new Text(success.Subscription.Name),
                                        new Text(success.Subscription.Description),
                                        new Text(success.Subscription.WebhookUrl),
                                        new Text(success.Subscription.ProvisioningState),
                                        new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Type))),
                                        new Text(success.Subscription.MaxDeliveryAttempts.ToString()),
                                        new Text(success.Subscription.DeadletterCount.ToString()),
                                        new Text(success.Subscription.Audience ?? string.Empty)
                                    );

                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscription", BaseCommands.CommandAction.Get, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetSubscriptionArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.TopicName) && string.IsNullOrWhiteSpace(arguments.SourceName))
        {
            return ValidationResult.Error("Either Topic name or Source name must be provided.");
        }

        if (string.IsNullOrWhiteSpace(arguments.SubscriptionName))
        {
            return ValidationResult.Error("Subscription name is required.");
        }

        return ValidationResult.Success();
    }
}