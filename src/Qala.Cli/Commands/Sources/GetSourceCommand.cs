using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class GetSourceCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetSourceArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetSourceArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new GetSourceRequest(argument.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Source", BaseCommands.CommandAction.Get, console);
                            console.Write(new Grid()
                                .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Source Type", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.Source.SourceId.ToString()),
                                    new Text(success.Source.Name),
                                    new Text(success.Source.Description),
                                    new Text(success.Source.SourceType.ToString())
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Source", BaseCommands.CommandAction.Get, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}