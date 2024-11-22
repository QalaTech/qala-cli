namespace Qala.Cli.Data.Models;

public class EventType
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Encoding { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = [];
}
