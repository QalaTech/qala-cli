using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Interfaces;

public interface IBaseTestExecution
{
    /// <summary>
    /// Execute the test
    /// </summary>
    /// <param name="input">The command to be execute</param>
    /// <param name="expectedSuccess">If the execution of the command will be successfull or not</param>
    /// <param name="expectedValidationResult">If validation fails, what messages are expected to appear, if it's expected to be successfull, then set as null</param>
    /// <param name="expectedConsoleOutput">What is the expected output</param>
    /// <returns></returns>
    Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput);

    /// <summary>
    /// Extract the expected output for a successful execution
    /// Since Spectre Console outputs everything in a grid, this method will be used to extract the expected output
    /// </summary>
    /// <param name="expectedOutput">Expected values to appear</param>
    /// <param name="expectedConsole">The TestConsole used to build the expected output</param>
    void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole);

    /// <summary>
    /// Extract the expected output for a failed execution
    /// Since Spectre Console outputs everything in a grid, this method will be used to extract the expected output
    /// </summary>
    /// <param name="expectedOutput">Expected values to appear</param>
    /// <param name="expectedConsole">The TestConsole used to build the expected output</param>
    void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole);
}