using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetWebhookSecretCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string TopicName, string SourceName, string SubscriptionName)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriptions secret inspect --topic TestTopic --subscription TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions secret i --topic TestTopic -s TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions secret inspect --source TestSource --subscription TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions secret i --source TestSource -s TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions secret i -s TestSubscription", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscriptions secret i --subscription TestSubscription", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscriptions secret i --topic TestTopic", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("subscriptions secret inspect --topic TestTopic --subscription NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions secret i --topic TestTopic -s NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions secret inspect --source TestSource --subscription NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions secret i --source TestSource -s NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetWebhookSecretCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "subscriptions secret inspect", null);
        var (TopicName, SourceName, SubscriptionName) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new GetWebhookSecretArgument()
        {
            TopicName = TopicName,
            SourceName = SourceName,
            SubscriptionName = SubscriptionName
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string TopicName, string SourceName, string SubscriptionName) ExtractArgumentsValues(List<string> arguments)
    {
        var topicNameIndex = arguments.IndexOf("--topic");
        var topicName = string.Empty;
        if (topicNameIndex != -1 && topicNameIndex + 1 < arguments.Count)
        {
            topicName = arguments[topicNameIndex + 1];
        }

        var sourceNameIndex = arguments.IndexOf("--source");
        var sourceName = string.Empty;
        if (sourceNameIndex != -1 && sourceNameIndex + 1 < arguments.Count)
        {
            sourceName = arguments[sourceNameIndex + 1];
        }

        var subscriptionNameIndex = arguments.IndexOf("--subscription") != -1 ? arguments.IndexOf("--subscription") : arguments.IndexOf("-s");
        var subscriptionName = string.Empty;
        if (subscriptionNameIndex != -1 && subscriptionNameIndex + 1 < arguments.Count)
        {
            subscriptionName = arguments[subscriptionNameIndex + 1];
        }

        return (topicName, sourceName, subscriptionName);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Webhook secret retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var webhookSecret = fixture.AvailableSubscriptions.FirstOrDefault(s => s.Name == expectedOutput[0])!.WebhookSecret;

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Webhook secret retrieved successfully:[/]");
        expectedConsole.MarkupLine($"[bold]{webhookSecret}[/]");
    }
}