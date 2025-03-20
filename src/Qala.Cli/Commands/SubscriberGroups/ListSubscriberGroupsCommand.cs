using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class ListSubscriberGroupsCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<ListSubscriberGroupsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListSubscriberGroupsArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new ListSubscriberGroupsRequest())
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscriber Groups", BaseCommands.CommandAction.List, console);
                            var grid = new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Key", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Topics", new Style(decoration: Decoration.Bold)),
                                    new Text("Audience", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var subscriberGroup in success.SubscriberGroups)
                            {
                                grid.AddRow(
                                    new Text(subscriberGroup.Key.ToString()),
                                    new Text(subscriberGroup.Name),
                                    new Text(subscriberGroup.Description),
                                    new Text(string.Join(", ", subscriberGroup.AvailablePermissions.Select(p => p.ResourceId))),
                                    new Text(subscriberGroup.Audience)
                                );
                            }

                            console.Write(grid);
                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscriber Group", BaseCommands.CommandAction.List, error.Message, console);

                            return -1;
                        }
                    );
            });
    }
}