using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class DeleteSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var topicName = fixture.AvailableSubscriptions.First().TopicName;
        var subscriptionId = fixture.AvailableSubscriptions.First().Id;
        var command = new DeleteSubscriptionCommand(fixture.Mediator);
        var arguments = new List<string> { "subscription", "delete", "-t", topicName, "-s", subscriptionId.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "delete", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new DeleteSubscriptionArgument() { TopicName = topicName, SubscriptionId = subscriptionId });
        
        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Subscription deleted successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new DeleteSubscriptionCommand(fixture.Mediator);
        var arguments = new List<string> { "subscription", "delete" };
        var context = new CommandContext(arguments, _remainingArguments, "delete", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new DeleteSubscriptionArgument());
        
        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during subscription deletion:", output);
    }
}