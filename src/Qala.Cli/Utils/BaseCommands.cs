using Spectre.Console;

namespace Qala.Cli.Utils;

public static class BaseCommands
{
    public static void DisplayLogoStart(string message, IAnsiConsole console)
    {
        console.Write(
            new FigletText("Qala CLI")
                .LeftJustified()
                .Color(Color.Yellow1));
        console.MarkupLine($"[yellow bold]{message}[/]");
    }

    public static void DisplaySuccessCommand(string entityName, CommandAction action, IAnsiConsole console)
    {
        switch (action)
        {
            case CommandAction.Create:
                console.MarkupLine($"[green bold]{entityName} created successfully:[/]");
                break;
            case CommandAction.Update:
                console.MarkupLine($"[green bold]{entityName} updated successfully:[/]");
                break;
            case CommandAction.Delete:
                console.MarkupLine($"[green bold]{entityName} deleted successfully.[/]");
                break;
            case CommandAction.Get:
                console.MarkupLine($"[green bold]{entityName} retrieved successfully:[/]");
                break;
            case CommandAction.List:
                console.MarkupLine($"[green bold]{entityName}:[/]");
                break;
            case CommandAction.Configuration:
                console.MarkupLine($"[green bold]{entityName} configured successfully:[/]");
                break;
            case CommandAction.Set:
                console.MarkupLine($"[green bold]{entityName} set successfully.[/]");
                break;
            case CommandAction.Rotate:
                console.MarkupLine($"[green bold]Rotation of {entityName} successfull:[/]");
                break;
            default:
                console.MarkupLine($"[green bold]{entityName}:[/]");
                break;
        }
    }

    public static void DisplayErrorCommand(string entityName, CommandAction action, string message, IAnsiConsole console)
    {
        switch (action)
        {
            case CommandAction.Create:
                console.MarkupLine($"[red bold]Error during {entityName} creation:[/]");
                break;
            case CommandAction.Update:
                console.MarkupLine($"[red bold]Error during {entityName} update:[/]");
                break;
            case CommandAction.Delete:
                console.MarkupLine($"[red bold]Error during {entityName} deletion:[/]");
                break;
            case CommandAction.Get:
                console.MarkupLine($"[red bold]Error during {entityName} retrieval:[/]");
                break;
            case CommandAction.List:
                console.MarkupLine($"[red bold]Error during {entityName} listing:[/]");
                break;
            case CommandAction.Configuration:
                console.MarkupLine($"[red bold]Error during {entityName} configuration:[/]");
                break;
            case CommandAction.Set:
                console.MarkupLine($"[red bold]Error setting {entityName}:[/]");
                break;
            case CommandAction.Rotate:
                console.MarkupLine($"[red bold]Error during rotation of {entityName}:[/]");
                break;
        }
        console.MarkupLine($"[red]{message}[/]");
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