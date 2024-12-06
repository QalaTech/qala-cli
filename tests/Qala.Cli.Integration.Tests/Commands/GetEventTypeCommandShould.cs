using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEventTypeCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new GetEventTypeCommand(fixture.Mediator, console);
        var arguments = new List<string> { "events", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                        expectedOutput.MarkupLine("[green bold]Event Type retrieved successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(5)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Type", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Content Type", new Style(decoration: Decoration.Bold)),
                                new Text("Categories", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text(fixture.AvailableEventTypes.First().Id.ToString()),
                                new Text(fixture.AvailableEventTypes.First().Type),
                                new Text(fixture.AvailableEventTypes.First().Description),
                                new Text(fixture.AvailableEventTypes.First().ContentType),
                                new Text(string.Join(", ", fixture.AvailableEventTypes.First().Categories))
                            )
                        );
                        if(!string.IsNullOrEmpty(fixture.AvailableEventTypes.First().Schema))
                        {
                            expectedOutput.Write(
                                new Panel(new JsonText(fixture.AvailableEventTypes.First().Schema))
                                    .Header("Schema")
                                    .Collapse()
                                    .RoundedBorder()
                                    .BorderColor(Color.Yellow)
                            );    
                        } 
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument() { Name = fixture.AvailableEventTypes.First().Type });

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
        var command = new GetEventTypeCommand(fixture.Mediator, console);
        var arguments = new List<string> { "events", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Event Type retrieval:[/]");
                expectedOutput.MarkupLine("[red]Invalid name[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument(){ Name = string.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}
