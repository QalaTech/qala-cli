using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface IOrganizationGateway
{
    Task<Organization?> GetOrganizationAsync();
}