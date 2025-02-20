using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ConfigCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Key, Guid EnvironmentId)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("config --k 297b5ae0-37c8-4419-b1f4-41f3d998d78e --e 55908371-b629-40b5-97bc-d6064cf8d3cd", true, null, new string[] { "297b5ae0-37c8-4419-b1f4-41f3d998d78e", "55908371-b629-40b5-97bc-d6064cf8d3cd" })]
    [InlineData("config --k 297b5ae0-37c8-4419-b1f4-41f3d998d78e", true, null, new string[] { "297b5ae0-37c8-4419-b1f4-41f3d998d78e", "" })]
    [InlineData("config --e 55908371-b629-40b5-97bc-d6064cf8d3cd", true, null, new string[] { "", "55908371-b629-40b5-97bc-d6064cf8d3cd" })]
    [InlineData("config", false, new string[] { "API Key or Environment ID are required" }, new string[] { "API Key or Environment ID are required" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new ConfigCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        var (key, environmentId) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new ConfigArgument()
        {
            Key = key,
            EnvironmentId = environmentId
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Configuration configured successfully:[/]");
        expectedConsole.Write(new Grid()
            .AddColumns(2)
            .AddRow(
                new Text("Api Key", new Style(decoration: Decoration.Bold)),
                new Text("Environment Id", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedOutput[0]),
                new Text(expectedOutput[1])
            ));
    }

    public (string Key, Guid EnvironmentId) ExtractArgumentsValues(List<string> arguments)
    {
        var keyIndex = arguments.IndexOf("--key") != -1 ? arguments.IndexOf("--key") : arguments.IndexOf("--k");
        var key = string.Empty;
        if (keyIndex != -1 && keyIndex + 1 < arguments.Count)
        {
            key = arguments[keyIndex + 1];
        }

        var envIndex = arguments.IndexOf("--env") != -1 ? arguments.IndexOf("--env") : arguments.IndexOf("--e");
        var environmentId = Guid.Empty;
        if (envIndex != -1 && envIndex + 1 < arguments.Count)
        {
            environmentId = Guid.Parse(arguments[envIndex + 1]);
        }

        return (key, environmentId);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Configuration configuration:[/]");
        expectedConsole.MarkupLine(expectedOutput[0]);
    }
}
