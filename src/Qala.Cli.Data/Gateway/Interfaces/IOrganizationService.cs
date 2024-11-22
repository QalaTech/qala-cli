using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface IOrganizationService
{
    Task<Organization> GetOrganizationAsync();
}