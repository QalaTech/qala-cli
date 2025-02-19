using Moq;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Environment;
using Spectre.Console;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class SetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment set --env 55908371-b629-40b5-97bc-d6064cf8d3cd")]
    public async Task Execute_WithValidParameters(string input)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new SetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[green bold]Environment set successfully.[/]");

        // Act
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = Guid.Parse(arguments[3]) });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }

    [Theory]
    [InlineData("environment set --env")]
    [InlineData("environment set")]
    [InlineData("environment set -e 55555371-b629-40b5-97bc-d6064cf8d3cd")]
    public async Task Execute_WithInvalidParameters(string input)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new SetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[red bold]Error setting Environment:[/]");
        expectedOutput.MarkupLine("Environment not valid");

        // Act
        var envIndex = arguments.IndexOf("--env") != -1 ? arguments.IndexOf("--env") : arguments.IndexOf("--e");
        Guid environmentId = Guid.Empty;
        if (envIndex != -1 && envIndex + 1 < arguments.Count)
        {
            environmentId = Guid.Parse(arguments[envIndex + 1]);
        }
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = environmentId });

        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }
}