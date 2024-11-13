using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands.Config;

public class CreateConfigCommand(IMediator mediator) : AsyncCommand<CreateConfigArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateConfigArgument config)
        => await mediator.Send(new CreateConfigRequest(config.Key, config.EnvironmentId))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.Write(
                        new FigletText("Qala CLI")
                            .LeftJustified()
                            .Color(Color.Yellow1));
                    AnsiConsole.MarkupLine($"[yellow bold]Config file[/]");

                    AnsiConsole.Write(new Grid()
                        .AddColumns(2)
                        .AddRow(
                            new Text("Api Key", new Style(decoration: Decoration.Bold)),
                            new Text("Environment Id", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(config.Key),
                            new Text(config.EnvironmentId)
                        )
                    );

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error creating API key[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
}