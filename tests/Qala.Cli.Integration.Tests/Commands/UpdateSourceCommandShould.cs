using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = fixture.AvailableSources.First().Name;
        var sourceNewName = "SourceNewName";
        var sourceDescription = "updated";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1/24", "127.0.0.1", "127.0.0.2" };
        var password = "password";
        var username = "username";
        var command = new UpdateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "update", sourceName, "-n", sourceNewName, "-d", sourceDescription, "-m", string.Join(",", methods), "-i", string.Join(",", whitelistedIpRanges), "-a", "basic", "--password", password, "--username", username };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[green bold]Source updated successfully:[/]");
                    });

        // Act
        var result = await command.ExecuteAsync(context, new UpdateSourceArgument() { Name = sourceName, NewName = sourceNewName, Description = sourceDescription, Methods = methods, IpWhitelisting = whitelistedIpRanges, Password = password, Username = username });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (var i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = fixture.AvailableSources.First().Name;
        var sourceDescription = "updated";
        var command = new UpdateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "update", sourceName, "-d", sourceDescription };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[red bold]Error during Source update:[/]");
                        expectedOutput.MarkupLine("Name is required");
                    });

        // Act
        var result = await command.ExecuteAsync(context, new UpdateSourceArgument() { Name = sourceName, Description = sourceDescription });

        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (var i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}