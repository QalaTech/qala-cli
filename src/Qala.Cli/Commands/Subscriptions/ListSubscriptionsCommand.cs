using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class ListSubscriptionsCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<ListSubscriptionsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListSubscriptionsArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new ListSubscriptionsRequest(arguments.TopicName, arguments.SourceName))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscriptions", BaseCommands.CommandAction.List, console);
                            var grid = success.Subscriptions.Select(s => string.IsNullOrWhiteSpace(s.Audience)).Length() == 0 ?
                            new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold))
                                ) :
                            new Grid()
                                .AddColumns(6)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold)),
                                    new Text("Audience", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var subscription in success.Subscriptions)
                            {
                                if (string.IsNullOrWhiteSpace(subscription.Audience))
                                {
                                    grid.AddRow(
                                        new Text(subscription.Id.ToString()),
                                        new Text(subscription.Name),
                                        new Text(subscription.Description),
                                        new Text(subscription.ProvisioningState),
                                        new Text(string.Join(", ", subscription.EventTypes.Select(et => et.Type)))
                                    );
                                }
                                else
                                {
                                    grid.AddRow(
                                        new Text(subscription.Id.ToString()),
                                        new Text(subscription.Name),
                                        new Text(subscription.Description),
                                        new Text(subscription.ProvisioningState),
                                        new Text(string.Join(", ", subscription.EventTypes.Select(et => et.Type))),
                                        new Text(subscription.Audience ?? string.Empty)
                                    );
                                }
                            }

                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscriptions", BaseCommands.CommandAction.List, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, ListSubscriptionsArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.TopicName) && string.IsNullOrWhiteSpace(arguments.SourceName))
        {
            return ValidationResult.Error("Either Topic name or Source name must be provided.");
        }

        return ValidationResult.Success();
    }
}