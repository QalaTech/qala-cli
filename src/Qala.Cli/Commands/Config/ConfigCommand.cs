using Qala.Cli.Utils;
using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Config;

public class ConfigCommand(IMediator mediator) : AsyncCommand<ConfigArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ConfigArgument argument)
    {
        return await mediator.Send(new ConfigRequest(argument.Key, argument.EnvironmentId))
            .ToAsync()
            .Match(
                success =>
                {
                    AnsiConsole.Write(new Grid()
                        .AddColumns(2)
                        .AddRow(
                            new Text("Api Key", new Style(decoration: Decoration.Bold)),
                            new Text("Environment Id", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(success.Config.Key),
                            new Text(success.Config.EnvironmentId.ToString())
                        )
                    );

                    return 0;
                },
                error =>
                {
                    AnsiConsole.MarkupLine($"[red bold]Error during configuration:[/]");
                    AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
    
    public override ValidationResult Validate(CommandContext context, ConfigArgument config)
    {
        if (string.IsNullOrWhiteSpace(config.Key))
        {
            return ValidationResult.Error("API Key is required");
        }

        return ValidationResult.Success();
    }
}