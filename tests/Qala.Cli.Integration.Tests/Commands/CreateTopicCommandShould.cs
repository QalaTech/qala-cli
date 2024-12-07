using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var topicName = "NewlyCreatedTestTopic";
        var topicDescription = "newly-created-topic-description";
        var eventTypeIds = fixture.AvailableEventTypes.Select(et => et.Id).ToList();
        var eventTypeNames = fixture.AvailableEventTypes.Select(et => et.Type).ToList();
        var command = new CreateTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topic", "create", "-n", topicName, "-d", topicDescription, "-e", string.Join(",", eventTypeNames) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine($"[green bold]Topic created successfully:[/]");
                expectedOutput.Write(new Grid()
                    .AddColumns(5)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                        new Text("Event Types", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                        new Text(topicName),
                        new Text(topicDescription),
                        new Text("Provisioning"),
                        new Text(string.Join(", ", eventTypeNames))
                    )
                );
            });
                
        // Act
        var result = await command.ExecuteAsync(context, new CreateTopicArgument() { Name = topicName, Description = topicDescription, EventTypeNames = eventTypeNames });
        
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
        var command = new CreateTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topic", "create" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine($"[red bold]Error during Topic creation:[/]");
                expectedOutput.MarkupLine($"[red]Name is required[/]");
            });
                
        // Act
        var result = await command.ExecuteAsync(context, new CreateTopicArgument());
        
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