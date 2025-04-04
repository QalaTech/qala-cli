using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class GetSourceCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GetSourceArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetSourceArgument argument)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                return await mediator.Send(new GetSourceRequest(argument.Name))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Source", BaseCommands.CommandAction.Get, console);
                            console.Write(new Grid()
                                .AddColumns(7)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Source Type", new Style(decoration: Decoration.Bold)),
                                    new Text("Http Methods", new Style(decoration: Decoration.Bold)),
                                    new Text("Authentication", new Style(decoration: Decoration.Bold)),
                                    new Text("Whitelisted IP Ranges", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.Source.SourceId.ToString()),
                                    new Text(success.Source.Name),
                                    new Text(success.Source.Description),
                                    new Text(success.Source.SourceType.ToString()),
                                    new Text(string.Join(", ", success.Source.Configuration.AllowedHttpMethods)),
                                    new Text(success.Source.Configuration.AuthenticationScheme!.Type.ToString()),
                                    new Text(string.Join(", ", success.Source.Configuration.WhitelistedIpRanges))
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Source", BaseCommands.CommandAction.Get, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, GetSourceArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Source name is required.");
        }

        return ValidationResult.Success();
    }
}