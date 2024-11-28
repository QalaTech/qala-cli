using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class SetEnvironmentCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<SetEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SetEnvironmentArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new SetEnvironmentRequest(argument.EnvironmentId))
                .ToAsync()
                .Match(
                    success => 
                    {
                        BaseCommands.DisplaySuccessMessage("Environment", BaseCommands.CommandAction.Set, console);
                        
                        return 0;
                    },
                    error => 
                    {
                        BaseCommands.DisplayErrorMessage("Environment", BaseCommands.CommandAction.Set, error.Message, console);

                        return -1;
                    }
                );
            });
    }
}