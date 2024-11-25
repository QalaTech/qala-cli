using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class GetWebhookSecretCommand(IMediator mediator) : AsyncCommand<GetWebhookSecretArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetWebhookSecretArgument settings)
    {
        return await mediator.Send(new GetWebhookSecretRequest(settings.TopicName, settings.SubscriptionId))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Webhook secret:[/]");
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

    public override ValidationResult Validate(CommandContext context, GetWebhookSecretArgument argument)
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