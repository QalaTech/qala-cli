using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new GetSubscriptionCommand(fixture.Mediator, console);
        var topicName = fixture.AvailableSubscriptions.First().TopicName;
        var subscriptionId = fixture.AvailableSubscriptions.First().Id;
        var arguments = new List<string> { "subscriptions", "inspect", "-t", topicName, "-s", subscriptionId.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Subscription retrieved successfully:[/]");
                var grid = new Grid()
                    .AddColumns(8)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                        new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                        new Text("Event Types", new Style(decoration: Decoration.Bold)),
                        new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                        new Text("Deadletters count", new Style(decoration: Decoration.Bold))
                    );

                grid.AddRow(
                    new Text(fixture.AvailableSubscriptions.First().Id.ToString()),
                    new Text(fixture.AvailableSubscriptions.First().Name),
                    new Text(fixture.AvailableSubscriptions.First().Description),
                    new Text(fixture.AvailableSubscriptions.First().WebhookUrl),
                    new Text(fixture.AvailableSubscriptions.First().ProvisioningState.ToString()),
                    new Text(string.Join(", ", fixture.AvailableSubscriptions.First().EventTypes.Select(et => et.Type))),
                    new Text(fixture.AvailableSubscriptions.First().MaxDeliveryAttempts.ToString()),
                    new Text(fixture.AvailableSubscriptions.First().DeadletterCount.ToString())
                );

                expectedOutput.Write(grid);
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetSubscriptionArgument() { TopicName = topicName, SubscriptionId = subscriptionId });

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
        var command = new GetSubscriptionCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscriptions", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Subscription retrieval:[/]");
                expectedOutput.MarkupLine("Topic name is required");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetSubscriptionArgument() { TopicName = string.Empty, SubscriptionId = Guid.Empty });

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