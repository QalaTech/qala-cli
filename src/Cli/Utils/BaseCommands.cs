using Spectre.Console;

namespace Qala.Cli.Utils;

public static class BaseCommands
{
    public static void DisplayStart(string message)
    {
        AnsiConsole.Write(
            new FigletText("Qala CLI")
                .LeftJustified()
                .Color(Color.Yellow1));
        AnsiConsole.MarkupLine($"[yellow bold]{message}[/]");
    }
}