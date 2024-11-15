using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Create;

public class CreateArgument : CommandSettings
{
    [CommandArgument(0, "[ENTITY]")]
    public string Entity { get; set; } = string.Empty;
}