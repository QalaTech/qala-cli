using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetWebhookSecretCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new GetWebhookSecretCommand(fixture.Mediator);
        var subscription = fixture.AvailableSubscriptions.First();
        var arguments = new List<string> { "subscriptions", "secret", "i", "-t", subscription.TopicName, "-s", subscription.Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "get-webhook-secret", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetWebhookSecretArgument() { TopicName = subscription.TopicName, SubscriptionId = subscription.Id });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Webhook secret retrieved successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new GetWebhookSecretCommand(fixture.Mediator);
        var arguments = new List<string> { "subscriptions", "secret", "i", "-t", "name", "-s", Guid.Empty.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "get-webhook-secret", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetWebhookSecretArgument() { TopicName = "name", SubscriptionId = Guid.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Webhook secret retrieval:", output);
    }
}