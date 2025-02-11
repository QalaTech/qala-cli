using MediatR;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.GenerateMarkdown;

public class GenerateMarkdownCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<GenerateMarkdownArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GenerateMarkdownArgument argument)
    {
        BaseCommands.DisplayQalaFiglet("Generate Markdown", console);
        return await mediator.Send(new GenerateMarkdownRequest())
            .ToAsync()
            .Match(
                success =>
                {
                    console.MarkupLine($"[green bold]Markdown generated successfully[/]");
                    return 0;
                },
                error =>
                {
                    console.MarkupLine($"[red bold]Error generating markdown[/]");
                    console.MarkupLine($"[red]{error.Message}[/]");

                    return -1;
                }
            );
    }
}