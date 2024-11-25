using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSubscriptionCommandShould(QalaCliBaseFixture qalaCliBaseFixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var topicName = "NewlyCreatedTestTopic";
        var subscriptionName = "NewlyCreatedTestSubscription";
        var subscriptionDescription = "newly-created-subscription-description";
        var webhookUrl = "https://newly-created-subscription-webhook.com";
        var eventTypeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var maxDeliveryAttempts = 5;
        var command = new CreateSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var arguments = new List<string> { "subscription", "create", topicName, "-n", subscriptionName, "-d", subscriptionDescription, "-w", webhookUrl, "-e", string.Join(",", eventTypeIds), "-m", maxDeliveryAttempts.ToString() };

        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new CreateSubscriptionArgument() { TopicName = topicName, Name = subscriptionName, Description = subscriptionDescription, WebhookUrl = webhookUrl, EventTypeIds = eventTypeIds, MaxDeliveryAttempts = maxDeliveryAttempts });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Subscription created successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new CreateSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var arguments = new List<string> { "subscription", "create" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new CreateSubscriptionArgument());

        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during subscription creation:", output);
    }
}