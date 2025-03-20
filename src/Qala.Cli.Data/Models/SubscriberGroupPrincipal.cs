namespace Qala.Cli.Data.Models;

public class SubscriberGroupPrincipal
{
    public Guid Key { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Permission> AvailablePermissions { get; set; } = new();
    public string Audience { get; set; } = string.Empty;
}