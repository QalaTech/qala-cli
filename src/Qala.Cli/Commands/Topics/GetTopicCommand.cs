using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class GetTopicCommand(IMediator mediator) : AsyncCommand<GetTopicArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetTopicArgument argument)
    {
        return await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new GetTopicRequest(argument.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessCommand("Topic", BaseCommands.CommandAction.Get);
                            AnsiConsole.Write(new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                                    new Text("Event Types", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.Topic.Id.ToString()),
                                    new Text(success.Topic.Name),
                                    new Text(success.Topic.Description),
                                    new Text(success.Topic.ProvisioningState),
                                    new Text(string.Join(", ", success.Topic.EventTypes))
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorCommand("Topic", BaseCommands.CommandAction.Get, error.Message);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetTopicArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        return ValidationResult.Success();
    }
}