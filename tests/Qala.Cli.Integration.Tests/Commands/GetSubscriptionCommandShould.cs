using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSubscriptionCommandShould(QalaCliBaseFixture qalaCliBaseFixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new GetSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var topicName = qalaCliBaseFixture.AvailableSubscriptions.First().TopicName;
        var subscriptionId = qalaCliBaseFixture.AvailableSubscriptions.First().Id;
        var arguments = new List<string> { "subscriptions", "inspect", "-t", topicName, "-s", subscriptionId.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetSubscriptionArgument() { TopicName = topicName, SubscriptionId = subscriptionId });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        var subscription = qalaCliBaseFixture.AvailableSubscriptions.First();
        Assert.Contains("Subscription retrieved successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new GetSubscriptionCommand(qalaCliBaseFixture.Mediator);
        var arguments = new List<string> { "subscriptions", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetSubscriptionArgument() { TopicName = string.Empty, SubscriptionId = Guid.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Subscription retrieval:", output);
    }
}