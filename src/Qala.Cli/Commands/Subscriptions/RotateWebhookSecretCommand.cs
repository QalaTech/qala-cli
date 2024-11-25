using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class RotateWebhookSecretCommand(IMediator mediator) : AsyncCommand<RotateWebhookSecretArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RotateWebhookSecretArgument settings)
    {
        return await mediator.Send(new RotateWebhookSecretRequest(settings.TopicName, settings.SubscriptionId))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Rotation of webhook secret successfull:[/]");
                    AnsiConsole.MarkupLine($"[bold]{success.WebhookSecret}[/]");

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
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