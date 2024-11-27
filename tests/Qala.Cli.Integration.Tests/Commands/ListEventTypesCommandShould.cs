using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListEventTypesCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new ListEventTypesCommand(fixture.Mediator, console);
        var arguments = new List<string> { "events", "list" };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Event Types:[/]");
                var grid = new Grid()
                    .AddColumns(4)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Type", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Content Type", new Style(decoration: Decoration.Bold))
                    );

                    foreach (var eventType in fixture.AvailableEventTypes)
                    {
                        grid.AddRow(
                            new Text(eventType.Id.ToString()),
                            new Text(eventType.Type),
                            new Text(eventType.Description),
                            new Text(eventType.ContentType)
                        );
                    }
                expectedOutput.Write(grid);
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new ListEventTypesArgument());

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
