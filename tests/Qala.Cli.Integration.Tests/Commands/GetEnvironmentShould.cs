using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEnvironmentShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetEnvironmentCommand(fixture.Mediator, console);
        var arguments = new List<string> { "environment", "current" };
        var context = new CommandContext(arguments, _remainingArguments, "current", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Environment retrieved successfully:[/]");
                expectedOutput.Write(new Grid()
                    .AddColumns(4)
                    .AddRow(
                        new Text("ID", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Region", new Style(decoration: Decoration.Bold)),
                        new Text("Type", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text(fixture.AvailableEnvironments.First().Id.ToString()),
                        new Text(fixture.AvailableEnvironments.First().Name),
                        new Text(fixture.AvailableEnvironments.First().Region),
                        new Text(fixture.AvailableEnvironments.First().EnvironmentType)
                    )
                );
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetEnvironmentArgument());

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}
