using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new GetTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topics", "name" };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Topic retrieved successfully:[/]");
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
                        new Text(fixture.AvailableTopics.First().Name),
                        new Text(fixture.AvailableTopics.First().Description),
                        new Text(fixture.AvailableTopics.First().ProvisioningState),
                        new Text(string.Join(", ", fixture.AvailableTopics.First().EventTypes.Select(et => et.Type)))
                    )
                );
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetTopicArgument() { Name = fixture.AvailableTopics.First().Name });

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
        var command = new GetTopicCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topics", "name" };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Topic retrieval:[/]");
                expectedOutput.MarkupLine("[red]Name is required[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetTopicArgument(){ Name = string.Empty });

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