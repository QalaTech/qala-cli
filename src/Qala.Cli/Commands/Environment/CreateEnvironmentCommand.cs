using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class CreateEnvironmentCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateEnvironmentArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new CreateEnvironmentRequest(argument.Name, argument.Region, argument.Type, argument.DisableSchemaValidation))
                .ToAsync()
                .Match(
                    success =>
                    {
                        BaseCommands.DisplaySuccessMessage("Environment", BaseCommands.CommandAction.Create, console);
                        console.Write(new Grid()
                            .AddColumns(5)
                            .AddRow(
                                new Text("ID", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Region", new Style(decoration: Decoration.Bold)),
                                new Text("Type", new Style(decoration: Decoration.Bold)),
                                new Text("Schema Validation Enabled", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text(success.Environment.Id.ToString()),
                                new Text(success.Environment.Name),
                                new Text(success.Environment.Region),
                                new Text(success.Environment.EnvironmentType),
                                new Text(success.Environment.IsSchemaValidationEnabled ? "True" : "False")
                            )
                        );

                        return 0;
                    },
                    error =>
                    {
                        BaseCommands.DisplayErrorMessage("Environment", BaseCommands.CommandAction.Create, error.Message, console);

                        return -1;
                    }
                );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateEnvironmentArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Name is required");
        }

        if (string.IsNullOrWhiteSpace(argument.Region))
        {
            return ValidationResult.Error("Region is required");
        }

        if (string.IsNullOrWhiteSpace(argument.Type))
        {
            return ValidationResult.Error("Type is required");
        }

        return ValidationResult.Success();
    }
}