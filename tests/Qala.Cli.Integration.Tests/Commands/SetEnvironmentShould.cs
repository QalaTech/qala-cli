using Moq;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Environment;
using Spectre.Console;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class SetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new SetEnvironmentCommand(fixture.Mediator, console);
        var arguments = new List<string> { "environment", "set", "-e", fixture.AvailableEnvironments.First().Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Environment set successfully.[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = fixture.AvailableEnvironments.First().Id });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new SetEnvironmentCommand(fixture.Mediator, console);
        var arguments = new List<string> { "environment", "set", "-e", Guid.NewGuid().ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "set", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error setting Environment:[/]");
                expectedOutput.MarkupLine("Environment not valid");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new SetEnvironmentArgument() { EnvironmentId = Guid.NewGuid() });

        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}