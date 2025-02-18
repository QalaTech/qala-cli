using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListSourcesCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new ListSourcesCommand(fixture.Mediator, console);
        var arguments = new List<string> { "sources", "list" };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine("[green bold]Sources:[/]");
                        var grid = new Grid()
                            .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Source Type", new Style(decoration: Decoration.Bold))
                                );

                        foreach (var source in fixture.AvailableSources)
                        {
                            grid.AddRow(
                                new Text(source.SourceId.ToString()),
                                new Text(source.Name),
                                new Text(source.Description),
                                new Text(source.SourceType.ToString())
                            );
                        }

                        expectedOutput.Write(grid);
                    });

        // Act
        var result = await command.ExecuteAsync(context, new ListSourcesArgument());

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (var i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}