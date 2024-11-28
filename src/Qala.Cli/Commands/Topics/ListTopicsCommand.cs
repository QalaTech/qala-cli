using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class ListTopicsCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<ListTopicsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListTopicsArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new ListTopicRequest())
                    .ToAsync()
                    .Match(
                        success => 
                        {
                            BaseCommands.DisplaySuccessMessage("Topics", BaseCommands.CommandAction.List, console);
                            var grid = new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var topic in success.Topics)
                            {
                                grid.AddRow(
                                    new Text(topic.Id.ToString()),
                                    new Text(topic.Name),
                                    new Text(topic.Description),
                                    new Text(topic.ProvisioningState),
                                    new Text(string.Join(", ", topic.EventTypes.Select(et => et.Type)))
                                );
                            }

                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Topics", BaseCommands.CommandAction.List, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}
