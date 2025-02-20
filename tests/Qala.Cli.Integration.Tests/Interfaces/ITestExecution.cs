namespace Qala.Cli.Integration.Tests.Interfaces;

public interface ITestExecution<T> : IBaseTestExecution
{
    /// <summary>
    /// Extract the values from the arguments of the specific commands
    /// </summary>
    /// <param name="arguments">The input command arguments</param>
    /// <returns></returns>
    T ExtractArgumentsValues(List<string> arguments);
}