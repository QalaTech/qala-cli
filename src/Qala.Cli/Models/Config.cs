namespace Qala.Cli.Models;

public class Config(
    string key,
    Guid environmentId)
{
    public string Key { get; set; } = key;
    public Guid EnvironmentId { get; set; } = environmentId;
}