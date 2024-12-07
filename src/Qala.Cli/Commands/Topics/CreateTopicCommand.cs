using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Topics;

public class CreateTopicCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateTopicArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateTopicArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new CreateTopicRequest(argument.Name, argument.Description, argument.EventTypeNames))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Topic", BaseCommands.CommandAction.Create, console);
                            console.Write(new Grid()
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
                                    new Text(string.Join(", ", success.Topic.EventTypes.Select(et => et.Type)))
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Topic", BaseCommands.CommandAction.Create, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateTopicArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Topic name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.Description))
        {
            return ValidationResult.Error("Topic description is required.");
        }

        if (argument.EventTypeNames.Count == 0)
        {
            return ValidationResult.Error("At least one event type name is required.");
        }

        return ValidationResult.Success();
    }
}