using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Subscriptions;

public class ListSubscriptionsCommand(IMediator mediator) : AsyncCommand<ListSubscriptionsArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListSubscriptionsArgument arguments)
    {
        return await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new ListSubscriptionsRequest(arguments.TopicName))
                    .ToAsync()
                    .Match(
                        success => 
                        {
                            BaseCommands.DisplaySuccessCommand("Subscriptions", BaseCommands.CommandAction.List);
                            var grid = new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var subscription in success.Subscriptions)
                            {
                                grid.AddRow(
                                    new Text(subscription.Id.ToString()),
                                    new Text(subscription.Name),
                                    new Text(subscription.Description),
                                    new Text(subscription.ProvisioningState),
                                    new Text(string.Join(", ", subscription.EventTypes.Select(et => et.Type)))
                                );
                            }

                            AnsiConsole.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorCommand("Subscriptions", BaseCommands.CommandAction.List, error.Message);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, ListSubscriptionsArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.TopicName))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        return ValidationResult.Success();
    }
}