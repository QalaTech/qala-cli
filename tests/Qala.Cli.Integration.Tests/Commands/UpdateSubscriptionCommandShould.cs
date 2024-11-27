using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSubscriptionCommandShould(QalaCliBaseFixture qalaCliBaseFixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var topicName = "NewlyUpdatedTestTopic";
        var subscriptionId = qalaCliBaseFixture.AvailableSubscriptions.First().Id;
        var subscriptionName = "NewlyUpdatedTestSubscription";
        var subscriptionDescription = "updated";
        var webhookUrl = "https://newly-updated-subscription-webhook.com";
        var eventTypeIds = qalaCliBaseFixture.AvailableEventTypes.Select(x => x.Id).ToList();
        var maxDeliveryAttempts = 5;
        var command = new UpdateSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var arguments = new List<string> { "subscription", "update", subscriptionId.ToString(), "-t", topicName, "-n", subscriptionName, "-d", subscriptionDescription, "-w", webhookUrl, "-e", string.Join(",", eventTypeIds), "-m", maxDeliveryAttempts.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new UpdateSubscriptionArgument() { TopicName = topicName, SubscriptionId = subscriptionId, Name = subscriptionName, Description = subscriptionDescription, WebhookUrl = webhookUrl, EventTypeIds = eventTypeIds, MaxDeliveryAttempts = maxDeliveryAttempts });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Subscription updated successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new UpdateSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var arguments = new List<string> { "subscription", "update" };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new UpdateSubscriptionArgument());

        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Subscription update:", output);
    }
}