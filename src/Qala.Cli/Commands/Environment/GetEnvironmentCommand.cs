using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class GetEnvironmentCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetEnvironmentArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new GetEnvironmentRequest())
                .ToAsync()
                .Match(
                    success =>
                    {
                        BaseCommands.DisplaySuccessMessage("Environment", BaseCommands.CommandAction.Get, console);
                        console.Write(new Grid()
                            .AddColumns(5)
                            .AddRow(
                                new Text("ID", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Region", new Style(decoration: Decoration.Bold)),
                                new Text("Type", new Style(decoration: Decoration.Bold)),
                                new Text("Schema Validation", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text(success.Environment.Id.ToString()),
                                new Text(success.Environment.Name),
                                new Text(success.Environment.Region),
                                new Text(success.Environment.EnvironmentType),
                                new Text(success.Environment.IsSchemaValidationEnabled ? "enabled" : "disabled")
                            )
                        );

                        return 0;
                    },
                    error =>
                    {
                        BaseCommands.DisplayErrorMessage("Environment", BaseCommands.CommandAction.Get, error.Message, console);

                        return -1;
                    }
                );
            });
    }
}