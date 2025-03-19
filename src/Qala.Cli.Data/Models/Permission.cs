namespace Qala.Cli.Data.Models;

public class Permission
{
    public string PermissionType { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
}