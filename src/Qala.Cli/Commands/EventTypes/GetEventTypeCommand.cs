using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace Qala.Cli.Commands.EventTypes;

public class GetEventTypeCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetEventTypeArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetEventTypeArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new GetEventTypeRequest(argument.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Event Type", BaseCommands.CommandAction.Get, console);
                            console.Write(new Grid()
                                .AddColumns(5)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Type", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Content Type", new Style(decoration: Decoration.Bold)),
                                    new Text("Categories", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.EventType.Id.ToString()),
                                    new Text(success.EventType.Type.ToString()),
                                    new Text(success.EventType.Description),
                                    new Text(success.EventType.ContentType),
                                    new Text(string.Join(", ", success.EventType.Categories))
                                )
                            );
                            
                            if(!string.IsNullOrEmpty(success.EventType.Schema))
                            {
                                console.Write(
                                    new Panel(new JsonText(success.EventType.Schema))
                                        .Header("Schema")
                                        .Collapse()
                                        .RoundedBorder()
                                        .BorderColor(Color.Yellow)
                                );    
                            }                        

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Event Type", BaseCommands.CommandAction.Get, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetEventTypeArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Event type Name is required");
        }

        return ValidationResult.Success();
    }
}
