using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateTopicCommandShould(QalaCliBaseFixture fixture): IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var topicName = fixture.AvailableTopics.First().Name;
        var topicDescription = "updated";
        var eventTypeIds = fixture.AvailableEventTypes.Select(et => et.Id).ToList();
        var command = new UpdateTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topic", "update", topicName, "-d", topicDescription, "-e", string.Join(",", eventTypeIds) };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Topic updated successfully:[/]");
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
                        new Text(fixture.AvailableTopics.First().Id.ToString()),
                        new Text(topicName),
                        new Text(topicDescription),
                        new Text(fixture.AvailableTopics.First().ProvisioningState),
                        new Text(string.Join(", ", eventTypeIds.Select(et => et.ToString())))
                    )
                );
            });
                
        // Act
        var result = await command.ExecuteAsync(context, new UpdateTopicArgument() { Name = topicName, Description = topicDescription, EventTypeIds = eventTypeIds });
        
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
        var command = new UpdateTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topic", "update" };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Topic update:[/]");
                expectedOutput.MarkupLine("[red]Name is required[/]");
            });
                
        // Act
        var result = await command.ExecuteAsync(context, new UpdateTopicArgument());
        
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