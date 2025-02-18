using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "sources", "name" };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[green bold]Source retrieved successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text(fixture.AvailableSources.First().SourceId.ToString()),
                                new Text(fixture.AvailableSources.First().Name),
                                new Text(fixture.AvailableSources.First().Description),
                                new Text(fixture.AvailableSources.First().SourceType.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new GetSourceArgument() { Name = fixture.AvailableSources.First().Name });

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