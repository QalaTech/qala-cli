using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class GetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment current", "55908371-b629-40b5-97bc-d6064cf8d3cd")]
    public async Task Execute_WithValidParameters_WithEnvironmentSetted(string input, string initialEnvId)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables(envId: initialEnvId);
        var console = new TestConsole();
        var command = new GetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "current", null);
        var expectedOutput = new TestConsole();
        var environment = string.IsNullOrWhiteSpace(initialEnvId) ? null : fixture.AvailableEnvironments.FirstOrDefault(e => e.Id == Guid.Parse(initialEnvId));
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[green bold]Environment retrieved successfully:[/]");
        if (environment != null)
        {
            expectedOutput.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("ID", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Region", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Schema Validation", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(environment.Id.ToString()),
                new Text(environment.Name),
                new Text(environment.Region),
                new Text(environment.EnvironmentType),
                new Text(environment.IsSchemaValidationEnabled ? "enabled" : "disabled")
            ));
        }

        // Act
        var result = await command.ExecuteAsync(context, new GetEnvironmentArgument());

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
    [InlineData("environment current", "")]
    public async Task Execute_WithValidParameters_NoEnvironmentSetted(string input, string initialEnvId)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables(envId: initialEnvId);
        var console = new TestConsole();
        var command = new GetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "current", null);
        var expectedOutput = new TestConsole();
        var environment = string.IsNullOrWhiteSpace(initialEnvId) ? null : fixture.AvailableEnvironments.FirstOrDefault(e => e.Id == Guid.Parse(initialEnvId));
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[red bold]Error during Environment retrieval:[/]");
        expectedOutput.MarkupLine("No environment was set");

        // Act
        var result = await command.ExecuteAsync(context, new GetEnvironmentArgument());

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
