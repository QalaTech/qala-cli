using MediatR;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Environment;

public class CreateEnvironmentCommand(IMediator mediator) : AsyncCommand<CreateEnvironmentArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateEnvironmentArgument argument)
    {
        return await mediator.Send(new CreateEnvironmentRequest(argument.Name, argument.Region, argument.Type))
        .ToAsync()
        .Match(
            success => 
            {
                AnsiConsole.MarkupLine($"[green bold]Environment created successfully[/]");
                AnsiConsole.Write(new Grid()
                    .AddColumns(4)
                    .AddRow(
                        new Text("ID", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Region", new Style(decoration: Decoration.Bold)),
                        new Text("Type", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text(success.Environment.Id.ToString()),
                        new Text(success.Environment.Name),
                        new Text(success.Environment.Region),
                        new Text(success.Environment.EnvironmentType)
                    )
                );
                
                return 0;
            },
            error => 
            {
                AnsiConsole.MarkupLine($"[red bold]Error creating environment[/]");
                AnsiConsole.MarkupLine($"[red]{error.Message}[/]");

                return -1;
            }
        );
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