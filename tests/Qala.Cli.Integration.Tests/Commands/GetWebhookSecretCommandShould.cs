using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetWebhookSecretCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole(); 
        var command = new GetWebhookSecretCommand(fixture.Mediator, console);
        var subscription = fixture.AvailableSubscriptions.First();
        var arguments = new List<string> { "subscriptions", "secret", "i", "-t", subscription.TopicName, "-s", subscription.Name };
        var context = new CommandContext(arguments, _remainingArguments, "get-webhook-secret", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[green bold]Webhook secret retrieved successfully:[/]");
                expectedOutput.MarkupLine($"[bold]{subscription.WebhookSecret}[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetWebhookSecretArgument() { TopicName = subscription.TopicName, SubscriptionName = subscription.Name });

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
        var command = new GetWebhookSecretCommand(fixture.Mediator, console);
        var arguments = new List<string> { "subscriptions", "secret", "i", "-t", "name", "-s", string.Empty };
        var context = new CommandContext(arguments, _remainingArguments, "get-webhook-secret", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx => 
                    {
                expectedOutput.MarkupLine("[red bold]Error during Webhook secret retrieval:[/]");
                expectedOutput.MarkupLine("Subscription name is required");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new GetWebhookSecretArgument() { TopicName = "name", SubscriptionName = string.Empty });

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