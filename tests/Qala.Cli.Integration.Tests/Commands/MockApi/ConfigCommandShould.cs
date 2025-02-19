using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class ConfigCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("config --k 297b5ae0-37c8-4419-b1f4-41f3d998d78e --e 55908371-b629-40b5-97bc-d6064cf8d3cd", "297b5ae0-37c8-4419-b1f4-41f3d998d78e", "55908371-b629-40b5-97bc-d6064cf8d3cd")]
    [InlineData("config --k 297b5ae0-37c8-4419-b1f4-41f3d998d78e", "297b5ae0-37c8-4419-b1f4-41f3d998d78e", "")]
    [InlineData("config --e 55908371-b629-40b5-97bc-d6064cf8d3cd", "", "55908371-b629-40b5-97bc-d6064cf8d3cd")]
    public async Task Execute_WithValidParameters(string input, string expectedApiKey, string expectedEnvironmentId)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new ConfigCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine($"[green bold]Configuration configured successfully:[/]");
        expectedOutput.Write(new Grid()
            .AddColumns(2)
            .AddRow(
                new Text("Api Key", new Style(decoration: Decoration.Bold)),
                new Text("Environment Id", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedApiKey),
                new Text(expectedEnvironmentId)
            ));

        // Act
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

        var result = await command.ExecuteAsync(
            context,
            new ConfigArgument()
            {
                Key = key,
                EnvironmentId = environmentId
            });

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
    [InlineData("config", "API Key or Environment ID are required")]
    public async Task Execute_WithInvalidParamenters(string input, string expectedErrorMessage)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new ConfigCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[red bold]Error during Configuration configuration:[/]");
        expectedOutput.MarkupLine(expectedErrorMessage);

        // Act
        var result = await command.ExecuteAsync(
            context,
            new ConfigArgument()
            {
                Key = string.Empty,
                EnvironmentId = Guid.Empty
            });

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
