using Spectre.Console;

namespace Qala.Cli.Utils;

public static class BaseCommands
{
    public static void DisplayLogoStart(string message)
    {
        AnsiConsole.Write(
            new FigletText("Qala CLI")
                .LeftJustified()
                .Color(Color.Yellow1));
        AnsiConsole.MarkupLine($"[yellow bold]{message}[/]");
    }

    public static void DisplaySuccessCommand(string entityName, CommandAction action)
    {
        switch (action)
        {
            case CommandAction.Create:
                AnsiConsole.MarkupLine($"[green bold]{entityName} created successfully:[/]");
                break;
            case CommandAction.Update:
                AnsiConsole.MarkupLine($"[green bold]{entityName} updated successfully:[/]");
                break;
            case CommandAction.Delete:
                AnsiConsole.MarkupLine($"[green bold]{entityName} deleted successfully.[/]");
                break;
            case CommandAction.Get:
                AnsiConsole.MarkupLine($"[green bold]{entityName} retrieved successfully:[/]");
                break;
            case CommandAction.List:
                AnsiConsole.MarkupLine($"[green bold]{entityName}:[/]");
                break;
            case CommandAction.Configuration:
                AnsiConsole.MarkupLine($"[green bold]{entityName} configured successfully:[/]");
                break;
            case CommandAction.Set:
                AnsiConsole.MarkupLine($"[green bold]{entityName} set successfully:[/]");
                break;
            case CommandAction.Rotate:
                AnsiConsole.MarkupLine($"[green bold]Rotation of {entityName} successfull:[/]");
                break;
            default:
                AnsiConsole.MarkupLine($"[green bold]{entityName}:[/]");
                break;
        }
    }

    public static void DisplayErrorCommand(string entityName, CommandAction action, string message)
    {
        switch (action)
        {
            case CommandAction.Create:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} creation:[/]");
                break;
            case CommandAction.Update:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} update:[/]");
                break;
            case CommandAction.Delete:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} deletion:[/]");
                break;
            case CommandAction.Get:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} retrieval:[/]");
                break;
            case CommandAction.List:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} listing:[/]");
                break;
            case CommandAction.Configuration:
                AnsiConsole.MarkupLine($"[red bold]Error during {entityName} configuration:[/]");
                break;
            case CommandAction.Set:
                AnsiConsole.MarkupLine($"[red bold]Error setting {entityName}:[/]");
                break;
            case CommandAction.Rotate:
                AnsiConsole.MarkupLine($"[red bold]Error during rotation of {entityName}:[/]");
                break;
        }
        AnsiConsole.MarkupLine($"[red]{message}[/]");
    }

    public enum CommandAction
    {
        Create,
        Update,
        Delete,
        Get,
        List,
        Configuration,
        Set,
        Rotate
    }
}