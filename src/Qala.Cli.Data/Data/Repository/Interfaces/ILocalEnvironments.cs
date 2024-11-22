namespace Qala.Cli.Data.Repository.Interfaces;

public interface ILocalEnvironments
{
    string GetLocalEnvironment(string variable);
    void SetLocalEnvironment(string variable, string? value);
}
