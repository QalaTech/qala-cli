using Moq;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string NewName, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    public Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        throw new NotImplementedException();
    }

    public (string Name, string NewName, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret) ExtractArgumentsValues(List<string> arguments)
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