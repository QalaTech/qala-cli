using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var topicName = "NewlyUpdatedTestTopic";
        var subscriptionId = fixture.AvailableSubscriptions.First().Id;
        var subscriptionName = fixture.AvailableSubscriptions.First().Name;
        var subscriptionNewName = "NewlyUpdatedTestSubscription";
        var subscriptionDescription = "updated";
        var webhookUrl = "https://newly-updated-subscription-webhook.com";
        var eventTypeNames = fixture.AvailableEventTypes.Select(x => x.Type).ToList();
        var eventTypeIds = fixture.AvailableEventTypes.Select(x => x.Id).ToList();
        var maxDeliveryAttempts = 5;
        var command = new UpdateSubscriptionCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscription", "update",subscriptionName, "-t", topicName, "-n", subscriptionNewName, "-d", subscriptionDescription, "-w", webhookUrl, "-e", string.Join(",", eventTypeNames), "-m", maxDeliveryAttempts.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Subscription updated successfully:[/]");
                expectedOutput.Write(new Grid()
                    .AddColumns(6)
                    .AddRow(
                        new Text("Id", new Style(decoration: Decoration.Bold)),
                        new Text("Name", new Style(decoration: Decoration.Bold)),
                        new Text("Description", new Style(decoration: Decoration.Bold)),
                        new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                        new Text("Event Types", new Style(decoration: Decoration.Bold)),
                        new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text(subscriptionId.ToString()),
                        new Text(subscriptionNewName),
                        new Text(subscriptionDescription),
                        new Text(webhookUrl),
                        new Text(string.Join(", ", eventTypeNames)),
                        new Text(maxDeliveryAttempts.ToString())
                    )
                );
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new UpdateSubscriptionArgument() { TopicName = topicName, SubscriptionName = subscriptionName, NewName = subscriptionNewName, Description = subscriptionDescription, WebhookUrl = webhookUrl, EventTypeNames = eventTypeNames, MaxDeliveryAttempts = maxDeliveryAttempts });

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
        var command = new UpdateSubscriptionCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscription", "update" };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Subscription update:[/]");
                expectedOutput.MarkupLine("[red]Topic name is required[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new UpdateSubscriptionArgument());

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