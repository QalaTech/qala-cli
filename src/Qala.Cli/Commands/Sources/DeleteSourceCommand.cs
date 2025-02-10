using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class DeleteSourceCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<DeleteSourceArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSourceArgument settings)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new DeleteSourceRequest(settings.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Source", BaseCommands.CommandAction.Delete, console);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Source", BaseCommands.CommandAction.Delete, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}