using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListSubscriptionsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new ListSubscriptionsCommand(fixture.Mediator, console);
        var topicName = fixture.AvailableSubscriptions.First().TopicName;
        var arguments = new List<string> { "subscriptions", "list", topicName };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Subscriptions:[/]");
                var grid = new Grid()
                    .AddColumns(5)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                        new Text("Event Types", new Style(decoration: Decoration.Bold))
                    );

                foreach (var subscription in fixture.AvailableSubscriptions)
                {
                    grid.AddRow(
                        new Text(subscription.Id.ToString()),
                        new Text(subscription.Name),
                        new Text(subscription.Description),
                        new Text(subscription.ProvisioningState),
                        new Text(string.Join(", ", subscription.EventTypes.Select(et => et.Type)))
                    );
                }

                expectedOutput.Write(grid);
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new ListSubscriptionsArgument() { TopicName = topicName});

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