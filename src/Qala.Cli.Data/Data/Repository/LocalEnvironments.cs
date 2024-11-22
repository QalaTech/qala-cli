using Qala.Cli.Data.Repository.Interfaces;

namespace Qala.Cli.Data.Repository;

public class LocalEnvironments : ILocalEnvironments
{
    public string GetLocalEnvironment(string variable)
    {
        return Environment.GetEnvironmentVariable(variable) ?? string.Empty;
    }

    public void SetLocalEnvironment(string variable, string? value)
    {
        Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
    }
}
