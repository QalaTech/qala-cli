using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class ListSourcesCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<ListSourcesArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListSourcesArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new ListSourcesRequest())
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Sources", BaseCommands.CommandAction.List, console);
                            var grid = new Grid()
                                .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Source Type", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var source in success.Sources)
                            {
                                grid.AddRow(
                                    new Text(source.Id.ToString()),
                                    new Text(source.Name),
                                    new Text(source.Description),
                                    new Text(source.SourceType.ToString())
                                );
                            }

                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Sources", BaseCommands.CommandAction.List, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}