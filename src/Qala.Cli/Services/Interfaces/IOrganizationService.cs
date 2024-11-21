using Qala.Cli.Models;

namespace Qala.Cli.Services.Interfaces;

public interface IOrganizationService
{
    Task<Organization> GetOrganizationAsync();
}