using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class RotateWebhookSecretCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new RotateWebhookSecretCommand(fixture.Mediator);
        var subscription = fixture.AvailableSubscriptions.First();
        var arguments = new List<string> { "subscriptions", "secret", "r", "-t", subscription.TopicName, "-s", subscription.Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "rotate-webhook-secret", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new RotateWebhookSecretArgument() { TopicName = subscription.TopicName, SubscriptionId = subscription.Id });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Rotation of Webhook secret successfull:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new RotateWebhookSecretCommand(fixture.Mediator);
        var arguments = new List<string> { "subscriptions", "secret", "r", "-t", "name", "-s", Guid.Empty.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "rotate-webhook-secret", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new RotateWebhookSecretArgument() { TopicName = "name", SubscriptionId = Guid.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during rotation of Webhook secret:", output);
    }
}