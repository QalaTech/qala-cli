using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string SubscriptionName, string TopicName, string SourceName, string NewName, string Description, string Webhook, List<string> EventTypeNames, int MaxDeliveryAttempts)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriptions update TestSubscription --topic TestTopic --name TestSubscriptionUpdated", true, null, new string[] { "TestSubscription", "", "TestSubscriptionUpdated", "", "", "", "" })]
    [InlineData("sub update TestSubscription2 --topic TestTopic -n TestSubscriptionUpdated", true, null, new string[] { "TestSubscription2", "", "TestSubscriptionUpdated", "", "", "", "" })]
    [InlineData("subscriptions update TestSubscription3 --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated", true, null, new string[] { "TestSubscription3", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "", "", "" })]
    [InlineData("sub update TestSubscription4 --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated", true, null, new string[] { "TestSubscription4", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "", "", "" })]
    [InlineData("subscriptions update TestSubscription5 --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io", true, null, new string[] { "TestSubscription5", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "" })]
    [InlineData("sub update TestSubscription6 --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io", true, null, new string[] { "TestSubscription6", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "" })]
    [InlineData("subscriptions update TestSubscription7 --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3", true, null, new string[] { "TestSubscription7", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "TestEvent1,TestEvent2,TestEvent3", "" })]
    [InlineData("sub update TestSubscription8 --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3", true, null, new string[] { "TestSubscription8", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "TestEvent1,TestEvent2,TestEvent3", "" })]
    [InlineData("subscriptions update TestSubscription9 --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5", true, null, new string[] { "TestSubscription9", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "TestEvent1,TestEvent2,TestEvent3", "5" })]
    [InlineData("sub update TestSubscription10 --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3 -m 5", true, null, new string[] { "TestSubscription10", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "TestEvent1,TestEvent2,TestEvent3", "5" })]
    [InlineData("subscriptions update TestSubscription11 --source TestSource --name TestSubscriptionUpdated", true, null, new string[] { "TestSubscription11", "", "TestSubscriptionUpdated", "", "", "", "" })]
    [InlineData("sub update TestSubscription12 --source TestSource -n TestSubscriptionUpdated", true, null, new string[] { "TestSubscription12", "", "TestSubscriptionUpdated", "", "", "", "" })]
    [InlineData("subscriptions update TestSubscription13 --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated", true, null, new string[] { "TestSubscription13", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "", "", "" })]
    [InlineData("sub update TestSubscription14 --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated", true, null, new string[] { "TestSubscription14", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "", "", "" })]
    [InlineData("subscriptions update TestSubscription15 --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io", true, null, new string[] { "TestSubscription15", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "" })]
    [InlineData("sub update TestSubscription16 --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io", true, null, new string[] { "TestSubscription16", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "" })]
    [InlineData("subscriptions update TestSubscription19 --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --max-attempts 5", true, null, new string[] { "TestSubscription19", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "5" })]
    [InlineData("sub update TestSubscription20 --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -m 5", true, null, new string[] { "TestSubscription20", "", "TestSubscriptionUpdated", "TestSubscriptionDescriptionUpdated", "https://test.webhook.io", "", "5" })]
    [InlineData("subscriptions update TestSubscription21 --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("sub update -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3 -m 5", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscriptions update --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("sub update --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3 -m 5", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("subscriptions update --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --max-attempts 5", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("sub update --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -m 5", false, new string[] { "Subscription name is required." }, new string[] { "Subscription name is required" })]
    [InlineData("subscriptions update NonExistingSubscription --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5", false, null, new string[] { "Subscription not found" })]
    [InlineData("sub update NonExistingSubscription --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3 -m 5", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions update NonExistingSubscription --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --max-attempts 5", false, null, new string[] { "Subscription not found" })]
    [InlineData("sub update NonExistingSubscription --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -m 5", false, null, new string[] { "Subscription not found" })]
    [InlineData("subscriptions update TestSubscription22 --topic TestTopic --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 12", false, null, new string[] { "Max delivery attempts should be a value between 0 and 10" })]
    [InlineData("sub update TestSubscription23 --topic TestTopic -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -e TestEvent1,TestEvent2,TestEvent3 -m 12", false, null, new string[] { "Max delivery attempts should be a value between 0 and 10" })]
    [InlineData("subscriptions update TestSubscription24 --source TestSource --name TestSubscriptionUpdated --description TestSubscriptionDescriptionUpdated --url https://test.webhook.io --max-attempts 12", false, null, new string[] { "Max delivery attempts should be a value between 0 and 10" })]
    [InlineData("sub update TestSubscription25 --source TestSource -n TestSubscriptionUpdated -d TestSubscriptionDescriptionUpdated -u https://test.webhook.io -m 12", false, null, new string[] { "Max delivery attempts should be a value between 0 and 10" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new UpdateSubscriptionCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var (SubscriptionName, TopicName, SourceName, NewName, Description, Webhook, EventTypeNames, MaxDeliveryAttempts) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new UpdateSubscriptionArgument()
        {
            SubscriptionName = SubscriptionName,
            TopicName = TopicName,
            SourceName = SourceName,
            NewName = NewName,
            Description = Description,
            WebhookUrl = Webhook,
            EventTypeNames = EventTypeNames,
            MaxDeliveryAttempts = MaxDeliveryAttempts
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string SubscriptionName, string TopicName, string SourceName, string NewName, string Description, string Webhook, List<string> EventTypeNames, int MaxDeliveryAttempts) ExtractArgumentsValues(List<string> arguments)
    {
        var subscriptionNameIndex = arguments.IndexOf("update");
        var subscriptionName = string.Empty;
        if (subscriptionNameIndex != -1 && subscriptionNameIndex + 1 < arguments.Count)
        {
            if (!arguments[subscriptionNameIndex + 1].StartsWith("-"))
            {
                subscriptionName = arguments[subscriptionNameIndex + 1];
            }
        }

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

        var newNameIndex = arguments.IndexOf("-n") != -1 ? arguments.IndexOf("-n") : arguments.IndexOf("--name");
        var newName = string.Empty;
        if (newNameIndex != -1 && newNameIndex + 1 < arguments.Count)
        {
            newName = arguments[newNameIndex + 1];
        }

        var descriptionIndex = arguments.IndexOf("-d") != -1 ? arguments.IndexOf("-d") : arguments.IndexOf("--description");
        var description = string.Empty;
        if (descriptionIndex != -1 && descriptionIndex + 1 < arguments.Count)
        {
            description = arguments[descriptionIndex + 1];
        }

        var webhookIndex = arguments.IndexOf("-u") != -1 ? arguments.IndexOf("-u") : arguments.IndexOf("--url");
        var webhook = string.Empty;
        if (webhookIndex != -1 && webhookIndex + 1 < arguments.Count)
        {
            webhook = arguments[webhookIndex + 1];
        }

        var eventsIndex = arguments.IndexOf("-e") != -1 ? arguments.IndexOf("-e") : arguments.IndexOf("--events");
        var events = new List<string>();
        if (eventsIndex != -1 && eventsIndex + 1 < arguments.Count)
        {
            events = arguments[eventsIndex + 1].Split(',').ToList();
        }

        var maxAttemptsIndex = arguments.IndexOf("-m") != -1 ? arguments.IndexOf("-m") : arguments.IndexOf("--max-attempts");
        var maxAttempts = 0;
        if (maxAttemptsIndex != -1 && maxAttemptsIndex + 1 < arguments.Count)
        {
            int.TryParse(arguments[maxAttemptsIndex + 1], out maxAttempts);
        }

        return (subscriptionName, topicName, sourceName, newName, description, webhook, events, maxAttempts);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Subscription update:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var currentSubscritpion = fixture.AvailableSubscriptions.FirstOrDefault(s => s.Name == expectedOutput[0]);

        var expectedSubscription = new Subscription()
        {
            Id = string.IsNullOrWhiteSpace(expectedOutput[1]) ? currentSubscritpion.Id : Guid.Parse(expectedOutput[1]),
            Name = string.IsNullOrWhiteSpace(expectedOutput[2]) ? currentSubscritpion.Name : expectedOutput[2],
            Description = string.IsNullOrWhiteSpace(expectedOutput[3]) ? currentSubscritpion.Description : expectedOutput[3],
            WebhookUrl = string.IsNullOrWhiteSpace(expectedOutput[4]) ? currentSubscritpion.WebhookUrl : expectedOutput[4],
            EventTypes = string.IsNullOrWhiteSpace(expectedOutput[5]) ? currentSubscritpion.EventTypes : expectedOutput[5].Split(',').Select(et => new EventType() { Type = et }).ToList(),
            MaxDeliveryAttempts = string.IsNullOrWhiteSpace(expectedOutput[6]) ? currentSubscritpion.MaxDeliveryAttempts : int.Parse(expectedOutput[6])
        };

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Subscription updated successfully.[/]");
        expectedConsole.Write(new Grid()
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
                new Text(expectedSubscription.Id.ToString()),
                new Text(expectedSubscription.Name),
                new Text(expectedSubscription.Description),
                new Text(expectedSubscription.WebhookUrl),
                new Text(string.Join(", ", expectedSubscription.EventTypes.Select(et => et.Type))),
                new Text(expectedSubscription.MaxDeliveryAttempts.ToString())
            )
        );
    }
}