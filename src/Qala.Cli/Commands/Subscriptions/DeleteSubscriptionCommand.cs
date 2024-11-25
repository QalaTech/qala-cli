using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class DeleteSubscriptionCommand(IMediator mediator) : AsyncCommand<DeleteSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSubscriptionArgument argument)
    {
        return await mediator.Send(new DeleteSubscriptionRequest(argument.TopicName, argument.SubscriptionId))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.MarkupLine($"[bold]Subscription deleted successfully:[/]");
                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during subscription deletion:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }

    public override ValidationResult Validate(CommandContext context, DeleteSubscriptionArgument argument)
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