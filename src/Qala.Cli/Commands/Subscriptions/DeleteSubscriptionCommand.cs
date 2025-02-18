using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class DeleteSubscriptionCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<DeleteSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSubscriptionArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new DeleteSubscriptionRequest(argument.TopicName, argument.SourceName, argument.SubscriptionName))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscription", BaseCommands.CommandAction.Delete, console);
                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscription", BaseCommands.CommandAction.Delete, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, DeleteSubscriptionArgument argument)
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