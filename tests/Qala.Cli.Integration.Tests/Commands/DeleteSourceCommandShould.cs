using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class DeleteSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = fixture.AvailableSources.First().Name;
        var command = new DeleteSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "delete", "-n", sourceName.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "delete", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[green bold]Source deleted successfully.[/]");
                    });

        // Act
        var result = await command.ExecuteAsync(context, new DeleteSourceArgument() { Name = sourceName });

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
        var command = new DeleteSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "delete" };
        var context = new CommandContext(arguments, _remainingArguments, "delete", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[red bold]Error during Source deletion:[/]");
                        expectedOutput.MarkupLine("Source name is required");
                    });

        // Act
        var result = await command.ExecuteAsync(context, new DeleteSourceArgument());

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