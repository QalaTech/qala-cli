using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var topicName = "NewlyCreatedTestTopic";
        var subscriptionName = "NewlyCreatedTestSubscription";
        var subscriptionDescription = "newly-created-subscription-description";
        var webhookUrl = "https://newly-created-subscription-webhook.com";
        var eventTypeIds = fixture.AvailableEventTypes.Select(et => et.Id).ToList();
        var eventTypeNames = fixture.AvailableEventTypes.Select(et => et.Type).ToList();
        var maxDeliveryAttempts = 5;
        var command = new CreateSubscriptionCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscription", "create", topicName, "-n", subscriptionName, "-d", subscriptionDescription, "-w", webhookUrl, "-e", string.Join(",", eventTypeNames), "-m", maxDeliveryAttempts.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);

        var expectedOutput = new TestConsole();
        expectedOutput.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("Processing request...", ctx => 
            {
                expectedOutput.MarkupLine($"[green bold]Subscription created successfully:[/]");
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
                        new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                        new Text(subscriptionName),
                        new Text(subscriptionDescription),
                        new Text(webhookUrl),
                        new Text(string.Join(", ", eventTypeIds.Select(et => et.ToString()))),
                        new Text(maxDeliveryAttempts.ToString())
                    )
                );
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateSubscriptionArgument() { TopicName = topicName, Name = subscriptionName, Description = subscriptionDescription, WebhookUrl = webhookUrl, EventTypeNames = eventTypeNames, MaxDeliveryAttempts = maxDeliveryAttempts });

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
        var command = new CreateSubscriptionCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscription", "create" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                        expectedOutput.MarkupLine($"[red bold]Error during Subscription creation:[/]");
                        expectedOutput.MarkupLine($"[red]Topic name is required[/]");
                    });
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateSubscriptionArgument());

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