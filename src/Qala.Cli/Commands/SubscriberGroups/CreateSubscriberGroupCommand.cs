using System.Text.RegularExpressions;
using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class CreateSubscriberGroupCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateSubscriberGroupArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateSubscriberGroupArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new CreateSubscriberGroupRequest(argument.Name, argument.Description, argument.Topics, argument.Audience))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Subscriber Group", BaseCommands.CommandAction.Create, console);
                            var grid = new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Key", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Topics", new Style(decoration: Decoration.Bold)),
                                    new Text("Audience", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.SubscriberGroup.Key.ToString()),
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
                            BaseCommands.DisplayErrorMessage("Subscriber Group", BaseCommands.CommandAction.Create, error.Message, console);
                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateSubscriberGroupArgument arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.Name))
        {
            return ValidationResult.Error("Subscriber Group name is required.");
        }

        if (string.IsNullOrWhiteSpace(arguments.Description))
        {
            return ValidationResult.Error("Subscriber Group description is required.");
        }

        if (arguments.Topics.Count == 0)
        {
            return ValidationResult.Error("At least one topic is required.");
        }

        if (!string.IsNullOrWhiteSpace(arguments.Audience) && !Regex.Match(arguments.Audience, ValidationHelper.AudiencesRegex, RegexOptions.IgnoreCase).Success)
        {
            return ValidationResult.Error("Audience must only contain alphanumerical values (A-Z, a-z, 0-9).");
        }

        return ValidationResult.Success();
    }
}