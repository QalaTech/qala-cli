using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new GetEnvironmentCommand(fixture.Mediator);
        var arguments = new List<string> { "environment", "current" };
        var context = new CommandContext(arguments, _remainingArguments, "current", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetEnvironmentArgument());

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Environment retrieved successfully:", output);
        Assert.Contains(fixture.AvailableEnvironments.First().Name, output);
        Assert.Contains(fixture.AvailableEnvironments.First().Id.ToString(), output);
        Assert.Contains(fixture.AvailableEnvironments.First().Region, output);
        Assert.Contains(fixture.AvailableEnvironments.First().EnvironmentType, output);
    }
}
