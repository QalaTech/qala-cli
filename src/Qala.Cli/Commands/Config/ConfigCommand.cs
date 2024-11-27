using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Config;

public class ConfigCommand(IMediator mediator) : AsyncCommand<ConfigArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ConfigArgument argument)
    {   
        return await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx => 
            {
                return await mediator.Send(new ConfigRequest(argument.Key, argument.EnvironmentId))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessCommand("Configuration", BaseCommands.CommandAction.Configuration);
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
                            BaseCommands.DisplayErrorCommand("Configuration", BaseCommands.CommandAction.Configuration, error.Message);

                            return -1;
                        }
                    );
            });
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