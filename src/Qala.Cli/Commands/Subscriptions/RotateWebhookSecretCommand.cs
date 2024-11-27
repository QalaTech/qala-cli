using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class RotateWebhookSecretCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<RotateWebhookSecretArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RotateWebhookSecretArgument settings)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new RotateWebhookSecretRequest(settings.TopicName, settings.SubscriptionId))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessCommand("Webhook secret", BaseCommands.CommandAction.Rotate, console);
                            console.MarkupLine($"[bold]{success.WebhookSecret}[/]");

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorCommand("Webhook secret", BaseCommands.CommandAction.Rotate, error.Message, console);

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

        if (argument.SubscriptionId == Guid.Empty)
        {
            return ValidationResult.Error("Subscription id is required.");
        }

        return ValidationResult.Success();
    }
}