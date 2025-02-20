using Spectre.Console;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests;

public class TestsUtils
{
    public static void AssertConsoleOutput(int result, bool expectedSuccess, string[] expectedOutput, TestConsole console, TestConsole expectedConsole)
    {
        Assert.Equal(expectedSuccess, result == 0);

        if (expectedOutput != null)
        {
            var actualLines = console.Lines;
            var expectedLines = expectedConsole.Lines;

            for (int i = 0; i < expectedLines.Count; i++)
            {
                Assert.Contains(expectedLines[i], actualLines);
            }
        }
    }

    public static void AssertValidationOutput(string[] expectedValidationResult, ValidationResult resultValidation)
    {
        if (expectedValidationResult != null && expectedValidationResult.Length > 0)
        {
            Assert.False(resultValidation.Successful);
            for (int i = 0; i < expectedValidationResult.Length; i++)
            {
                Assert.Contains(expectedValidationResult[i], resultValidation.Message);
            }
        }
        else
        {
            Assert.True(resultValidation.Successful);
        }
    }
}