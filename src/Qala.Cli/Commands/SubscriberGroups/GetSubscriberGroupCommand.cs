using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class GetSubscriberGroupCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetSubscriberGroupArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetSubscriberGroupArgument arguments)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new GetSubscriberGroupRequest(arguments.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscriber Group", BaseCommands.CommandAction.Get, console);
                            var grid = new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Topics", new Style(decoration: Decoration.Bold)),
                                    new Text("Audience", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.SubscriberGroup.Id.ToString()),
                                    new Text(success.SubscriberGroup.Name),
                                    new Text(success.SubscriberGroup.Description),
                                    new Text(string.Join(", ", success.SubscriberGroup.AvailablePermissions.Select(p => p.ResourceId))),
                                    new Text(success.SubscriberGroup.Audience)
                                );
                            console.Write(grid);
                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Subscriber Group", BaseCommands.CommandAction.Get, error.Message, console);
                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetSubscriberGroupArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.Name))
        {
            return ValidationResult.Error("Subscriber Group name is required.");
        }

        return ValidationResult.Success();
    }
}