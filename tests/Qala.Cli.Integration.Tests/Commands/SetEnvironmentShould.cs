using Moq;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Environment;
using Spectre.Console;
using Qala.Cli.Integration.Tests.Fixtures;

namespace Qala.Cli.Integration.Tests.Commands;

public class SetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new SetEnvironmentCommand(fixture.Mediator);
        var arguments = new List<string> { "environment", "set", "-e", fixture.AvailableEnvironments.First().Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = fixture.AvailableEnvironments.First().Id });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Environment set successfully", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new SetEnvironmentCommand(fixture.Mediator);
        var arguments = new List<string> { "environment", "set", "-e", Guid.NewGuid().ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = Guid.NewGuid() });

        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error setting environment", output);
    }
}