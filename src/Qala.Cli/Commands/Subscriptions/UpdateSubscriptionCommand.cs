using System.Text.RegularExpressions;
using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class UpdateSubscriptionCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<UpdateSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, UpdateSubscriptionArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new UpdateSubscriptionRequest(argument.TopicName, argument.SourceName, argument.SubscriptionName, argument.NewName, argument.Description, argument.WebhookUrl, argument.EventTypeNames, argument.MaxDeliveryAttempts, argument.Audience))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscription", BaseCommands.CommandAction.Update, console);
                            var grid = string.IsNullOrWhiteSpace(argument.TopicName) ?
                            new Grid()
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
                                ) :
                            new Grid()
                                .AddColumns(7)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold)),
                                    new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                                    new Text("Audience", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.Subscription.Id.ToString()),
                                    new Text(success.Subscription.Name),
                                    new Text(success.Subscription.Description),
                                    new Text(success.Subscription.WebhookUrl),
                                    new Text(string.Join(", ", success.Subscription.EventTypes.Select(et => et.Type))),
                                    new Text(success.Subscription.MaxDeliveryAttempts.ToString()),
                                    new Text(success.Subscription.Audience ?? string.Empty)
                                );

                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscription", BaseCommands.CommandAction.Update, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, UpdateSubscriptionArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.TopicName) && string.IsNullOrWhiteSpace(argument.SourceName))
        {
            return ValidationResult.Error("Either Topic name or Source name must be provided.");
        }

        if (string.IsNullOrWhiteSpace(argument.SubscriptionName))
        {
            return ValidationResult.Error("Subscription name is required.");
        }

        if (!string.IsNullOrWhiteSpace(argument.Audience) && !Regex.Match(argument.Audience, ValidationHelper.AudiencesRegex, RegexOptions.IgnoreCase).Success)
        {
            return ValidationResult.Error("Audience entries must only contain alphanumerical values (A-Z, a-z, 0-9)");
        }

        return ValidationResult.Success();
    }
}