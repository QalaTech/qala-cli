using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.EventTypes;

public class CreateEventTypesCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateEventTypeArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateEventTypeArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new CreateEventTypesRequest(argument.ImportFilePath))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Event Types", BaseCommands.CommandAction.Create, console);
                            var grid = new Grid()
                                .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Type", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Content Type", new Style(decoration: Decoration.Bold))
                                );

                            foreach (var eventType in success.EventTypes)
                            {
                                grid.AddRow(
                                    new Text(eventType.Id.ToString()),
                                    new Text(eventType.Type),
                                    new Text(eventType.Description),
                                    new Text(eventType.ContentType)
                                );
                            }
                            console.Write(grid);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Event Types", BaseCommands.CommandAction.Create, error.Message, console);
                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateEventTypeArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.ImportFilePath))
        {
            return ValidationResult.Error("Import file path is required.");
        }

        var allowedExtensions = new[] { ".yaml", ".yml", ".json" };
        var fileExtension = Path.GetExtension(argument.ImportFilePath);

        if (!allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return ValidationResult.Error("The import file must have a .yaml, .yml, or .json extension.");
        }

        return ValidationResult.Success();
    }
}