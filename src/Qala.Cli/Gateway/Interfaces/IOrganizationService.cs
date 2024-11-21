using Qala.Cli.Models;

namespace Qala.Cli.Gateway.Interfaces;

public interface IOrganizationService
{
    Task<Organization> GetOrganizationAsync();
}