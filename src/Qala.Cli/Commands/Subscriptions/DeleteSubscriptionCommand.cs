using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class DeleteSubscriptionCommand(IMediator mediator) : AsyncCommand<DeleteSubscriptionArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSubscriptionArgument argument)
    {
        return await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new DeleteSubscriptionRequest(argument.TopicName, argument.SubscriptionId))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessCommand("Subscription", BaseCommands.CommandAction.Delete);
                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorCommand("Subscription", BaseCommands.CommandAction.Delete, error.Message);

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

        if (argument.SubscriptionId == Guid.Empty)
        {
            return ValidationResult.Error("Subscription id is required.");
        }

        return ValidationResult.Success();
    }
}