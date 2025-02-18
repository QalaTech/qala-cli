using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class RotateWebhookSecretCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<RotateWebhookSecretArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RotateWebhookSecretArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new RotateWebhookSecretRequest(arguments.TopicName, arguments.SourceName, arguments.SubscriptionName))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Webhook secret", BaseCommands.CommandAction.Rotate, console);
                            console.MarkupLine($"[bold]{success.WebhookSecret}[/]");

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Webhook secret", BaseCommands.CommandAction.Rotate, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, RotateWebhookSecretArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.TopicName))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.SubscriptionName))
        {
            return ValidationResult.Error("Subscription name is required.");
        }

        return ValidationResult.Success();
    }
}