using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string TopicName, string SourceName, string SubscriptionName)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriptions inspect --topic TestTopic --subscription TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions i --topic TestTopic -s TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions inspect --source TestSource --subscription TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions i --source TestSource -s TestSubscription", true, null, new string[] { "TestSubscription" })]
    [InlineData("subscriptions inspect --subscription TestSubscription", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscriptions i --subscription TestSubscription", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscriptions inspect --topic TestTopic", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("subscriptions inspect --source TestSource", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("subscriptions inspect --topic TestTopic --subscription NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions inspect --source TestSource --subscription NonExistingSubscription", false, null, new string[] { "Subscription not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetSubscriptionCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
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

        var inputArguments = new GetSubscriptionArgument()
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
        expectedConsole.MarkupLine($"[red bold]Error during Subscription retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedSubscription = fixture.AvailableSubscriptions.FirstOrDefault(s => s.Name == expectedOutput[0]);

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Subscription retrieved successfully:[/]");
        var grid = new Grid()
            .AddColumns(8)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                new Text("Event Types", new Style(decoration: Decoration.Bold)),
                new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                new Text("Deadletters count", new Style(decoration: Decoration.Bold))
            );

        grid.AddRow(
            new Text(expectedSubscription!.Id.ToString()),
            new Text(expectedSubscription.Name),
            new Text(expectedSubscription.Description),
            new Text(expectedSubscription.WebhookUrl),
            new Text(expectedSubscription.ProvisioningState),
            new Text(string.Join(", ", expectedSubscription.EventTypes.Select(et => et.Type))),
            new Text(expectedSubscription.MaxDeliveryAttempts.ToString()),
            new Text(expectedSubscription.DeadletterCount.ToString())
        );

        expectedConsole.Write(grid);
    }
}