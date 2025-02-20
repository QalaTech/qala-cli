using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class SetEnvironmentCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<Guid>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment set --env 55908371-b629-40b5-97bc-d6064cf8d3cd", true, null, new string[] { "Environment set successfully." })]
    [InlineData("environment set --env", false, new string[] { "Environment ID is required" }, new string[] { "Environment ID is required" })]
    [InlineData("environment set", false, new string[] { "Environment ID is required" }, new string[] { "Environment ID is required" })]
    [InlineData("environment set -e 55555371-b629-40b5-97bc-d6064cf8d3cd", false, null, new string[] { "Environment not valid" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new SetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "current", null);
        var envId = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new SetEnvironmentArgument()
        {
            EnvironmentId = envId
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public Guid ExtractArgumentsValues(List<string> arguments)
    {
        var envIdIndex = arguments.IndexOf("--env") != -1 ? arguments.IndexOf("--env") : arguments.IndexOf("-e");
        var envId = Guid.Empty;
        if (envIdIndex != -1 && envIdIndex + 1 < arguments.Count)
        {
            Guid.TryParse(arguments[envIdIndex + 1], out envId);
        }

        return envId;
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error setting Environment:[/]");
        expectedConsole.MarkupLine(expectedOutput[0]);
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]{expectedOutput[0]}[/]");
    }
}