using Moq;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class DeleteSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<string>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    public Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        throw new NotImplementedException();
    }

    public string ExtractArgumentsValues(List<string> arguments)
    {
        throw new NotImplementedException();
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        throw new NotImplementedException();
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        throw new NotImplementedException();
    }
}