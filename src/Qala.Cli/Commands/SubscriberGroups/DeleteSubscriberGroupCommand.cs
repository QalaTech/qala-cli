using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class DeleteSubscriberGroupCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<DeleteSubscriberGroupArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSubscriberGroupArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new DeleteSubscriberGroupRequest(argument.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscriber Group", BaseCommands.CommandAction.Delete, console);
                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscriber Group", BaseCommands.CommandAction.Delete, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, DeleteSubscriberGroupArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.Name))
        {
            return ValidationResult.Error("Subscriber Group name is required.");
        }

        return ValidationResult.Success();
    }
}