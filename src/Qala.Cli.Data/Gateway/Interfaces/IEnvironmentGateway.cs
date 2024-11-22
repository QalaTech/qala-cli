namespace Qala.Cli.Data.Gateway.Interfaces;

public interface IEnvironmentGateway
{
    Task<Models.Environment> CreateEnvironmentAsync(Models.Environment environment);
}
