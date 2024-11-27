using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListTopicsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new ListTopicsCommand(fixture.Mediator, console);
        var arguments = new List<string> { "topics", "list" };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Topics:[/]");
                var grid = new Grid()
                    .AddColumns(5)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                        new Text("Event Types", new Style(decoration: Decoration.Bold))
                    );

                foreach (var topic in fixture.AvailableTopics)
                {
                    grid.AddRow(
                        new Text(topic.Id.ToString()),
                        new Text(topic.Name),
                        new Text(topic.Description),
                        new Text(topic.ProvisioningState),
                        new Text(string.Join(", ", topic.EventTypes.Select(et => et.Type)))
                    );
                }

                expectedOutput.Write(grid);
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new ListTopicsArgument());

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
