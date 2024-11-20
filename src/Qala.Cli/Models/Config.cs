namespace Qala.Cli.Models;

public class Config(
    string apiKey,
    string environmentId)
{
    public string ApiKey { get; set; } = apiKey;
    public string EnvironmentId { get; set; } = environmentId;
}