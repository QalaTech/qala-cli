using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Get;

public class GetArgument : CommandSettings
{
    [CommandArgument(0, "[ENTITY]")]
    public string Entity { get; set; } = string.Empty;
}