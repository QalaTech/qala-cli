using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetWebhookSecretCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetWebhookSecretArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetWebhookSecretArgument settings)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new GetWebhookSecretRequest(settings.TopicName, settings.SubscriptionName))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Webhook secret", BaseCommands.CommandAction.Get, console);
                            console.MarkupLine($"[bold]{success.WebhookSecret}[/]");

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Webhook secret", BaseCommands.CommandAction.Get, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetWebhookSecretArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.TopicName))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (argument.SubscriptionName == string.Empty)
        {
            return ValidationResult.Error("Subscription name is required.");
        }

        return ValidationResult.Success();
    }
}