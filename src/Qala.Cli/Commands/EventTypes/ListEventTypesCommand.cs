using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class ListEventTypesCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<ListEventTypesArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListEventTypesArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new ListEventTypesRequest())
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Event Types", BaseCommands.CommandAction.List, console);
                            var grid = new Grid()
                                .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Type", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Content Type", new Style(decoration: Decoration.Bold))
                                );

                                foreach (var eventType in success.EventTypes)
                                {
                                    grid.AddRow(
                                        new Text(eventType.Id.ToString()),
                                        new Text(eventType.Type),
                                        new Text(eventType.Description),
                                        new Text(eventType.ContentType)
                                    );
                                }
                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Event Types", BaseCommands.CommandAction.List, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}
