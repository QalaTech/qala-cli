using Cli.Utils;
using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands.Config;

public class ConfigCommand(IMediator mediator) : AsyncCommand<ConfigArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ConfigArgument config)
        => await mediator.Send(new ConfigRequest(config.Key))
            .ToAsync()
            .Match(
                success =>
                {
                    BaseCommands.DisplayStart("Create Config");

                    AnsiConsole.Write(new Grid()
                        .AddColumns(1)
                        .AddRow(
                            new Text("Api Key", new Style(decoration: Decoration.Bold))
                        )
                        .AddRow(
                            new Text(config.Key)
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
    public override ValidationResult Validate(CommandContext context, ConfigArgument config)
    {
        if (string.IsNullOrWhiteSpace(config.Key))
        {
            return ValidationResult.Error("API Key is required");
        }

        return ValidationResult.Success();
    }
}