namespace Qala.Cli.Models;

public class Environment
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string EnvironmentType { get; set; } = string.Empty;
}