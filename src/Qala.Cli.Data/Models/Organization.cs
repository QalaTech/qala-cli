namespace Qala.Cli.Data.Models;

public class Organization()
{
    public string Name { get; set; } = string.Empty;
    public string SubDomain { get; set; } = string.Empty;
    public List<Environment> Environments { get; set; } = [];
}