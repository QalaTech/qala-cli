using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSubscriptionCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string TopicName, string SourceName, string Name, string Description, string WebhookUrl, List<string> EventTypeNames, int MaxDeliveryAttempts, string Audience)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    private bool isTopicSubscription { get; set; }

    [Theory]
    [InlineData("subscription create --topic TestTopic1 --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscription", "newly-subscription-description", "http://test.com", "TestEvent1,TestEvent2,TestEvent3", "5", "" })]
    [InlineData("subscription create --topic TestTopic2 --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5 -a audience", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscription", "newly-subscription-description", "http://test.com", "TestEvent1,TestEvent2,TestEvent3", "5", "audience" })]
    [InlineData("subscription create --topic TestTopic3 --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --events TestEvent1,TestEvent2,TestEvent3 --max-attempts 5 --audience audience", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscription", "newly-subscription-description", "http://test.com", "TestEvent1,TestEvent2,TestEvent3", "5", "audience" })]
    [InlineData("subscription create --source TestSource --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --max-attempts 5", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscription", "newly-subscription-description", "http://test.com", "TestEvent1,TestEvent2,TestEvent3", "5" })]
    [InlineData("subscription create --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --max-attempts 5", false, new string[] { "Either Topic name or Source name must be provided." }, new string[] { "Either Topic name or Source name must be provided" })]
    [InlineData("subscription create --source TestSource --description newly-subscription-description --url http://test.com --max-attempts 5", false, new string[] { "Subscription name is required." }, new string[] { "Name is required" })]
    [InlineData("subscription create --source TestSource --name NewlyCreatedTestSubscription --url http://test.com --max-attempts 5", false, new string[] { "Subscription description is required." }, new string[] { "Description is required" })]
    [InlineData("subscription create --source TestSource --name NewlyCreatedTestSubscription --description newly-subscription-description --max-attempts 5", false, new string[] { "Webhook URL is required." }, new string[] { "Webhook url is required" })]
    [InlineData("subscription create --topic TestTopic --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --max-attempts 5", false, new string[] { "At least one event type name is required." }, new string[] { "Event type ids are required" })]
    [InlineData("subscription create --source TestSource --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --max-attempts -1", false, new string[] { "Max delivery attempts should be between 0 and 10." }, new string[] { "Max delivery attempts should be between 0 and 10" })]
    [InlineData("subscription create --source TestSource --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --max-attempts 11", false, new string[] { "Max delivery attempts should be between 0 and 10." }, new string[] { "Max delivery attempts should be between 0 and 10" })]
    [InlineData("subscription create --topic TestTopic --name NewlyCreatedTestSubscription --description newly-subscription-description --url http://test.com --events NonExistingEvents --max-attempts 5", false, null, new string[] { "Event types not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateSubscriptionCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var (topicName, sourceName, name, description, webhookUrl, eventTypeNames, maxDeliveryAttempts, audience) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            isTopicSubscription = string.IsNullOrWhiteSpace(sourceName);
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new CreateSubscriptionArgument()
        {
            TopicName = topicName,
            SourceName = sourceName,
            Name = name,
            Description = description,
            WebhookUrl = webhookUrl,
            EventTypeNames = eventTypeNames,
            MaxDeliveryAttempts = maxDeliveryAttempts,
            Audience = audience
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string TopicName, string SourceName, string Name, string Description, string WebhookUrl, List<string> EventTypeNames, int MaxDeliveryAttempts, string Audience) ExtractArgumentsValues(List<string> arguments)
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

        var nameIndex = arguments.IndexOf("--name") != -1 ? arguments.IndexOf("--name") : arguments.IndexOf("-n");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            name = arguments[nameIndex + 1];
        }

        var descriptionIndex = arguments.IndexOf("--description") != -1 ? arguments.IndexOf("--description") : arguments.IndexOf("-d");
        var description = string.Empty;
        if (descriptionIndex != -1 && descriptionIndex + 1 < arguments.Count)
        {
            description = arguments[descriptionIndex + 1];
        }

        var webhookUrlIndex = arguments.IndexOf("--url") != -1 ? arguments.IndexOf("--url") : arguments.IndexOf("-u");
        var webhookUrl = string.Empty;
        if (webhookUrlIndex != -1 && webhookUrlIndex + 1 < arguments.Count)
        {
            webhookUrl = arguments[webhookUrlIndex + 1];
        }

        var eventTypeIndex = arguments.IndexOf("--events") != -1 ? arguments.IndexOf("--events") : arguments.IndexOf("-e");
        var eventTypeNames = new List<string>();
        if (eventTypeIndex != -1 && eventTypeIndex + 1 < arguments.Count)
        {
            eventTypeNames = arguments[eventTypeIndex + 1].Split(',').ToList();
        }

        var maxDeliveryAttemptsIndex = arguments.IndexOf("--max-attempts") != -1 ? arguments.IndexOf("--max-attempts") : arguments.IndexOf("-m");
        var maxDeliveryAttempts = 0;
        if (maxDeliveryAttemptsIndex != -1 && maxDeliveryAttemptsIndex + 1 < arguments.Count)
        {
            maxDeliveryAttempts = int.Parse(arguments[maxDeliveryAttemptsIndex + 1]);
        }

        var audienceIndex = arguments.IndexOf("--audience") != -1 ? arguments.IndexOf("--audience") : arguments.IndexOf("-a");
        var audience = string.Empty;
        if (audienceIndex != -1 && audienceIndex + 1 < arguments.Count)
        {
            audience = arguments[audienceIndex + 1];
        }

        return (topicName, sourceName, name, description, webhookUrl, eventTypeNames, maxDeliveryAttempts, audience);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Subscription creation:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedSubscription = new Subscription()
        {
            Id = Guid.Parse(expectedOutput[0]),
            Name = expectedOutput[1],
            Description = expectedOutput[2],
            WebhookUrl = expectedOutput[3],
            EventTypes = expectedOutput[4].Split(',').Select(et => new EventType() { Type = et }).ToList(),
            MaxDeliveryAttempts = int.Parse(expectedOutput[5]),
            Audience = expectedOutput.Length == 7 ? expectedOutput[6] : string.Empty
        };

        expectedConsole.MarkupLine($"[green]Subscription created successfully:[/]");
        expectedConsole.MarkupLine("Processing request...");
        var grid = isTopicSubscription ?
            new Grid()
                .AddColumns(7)
                .AddRow(
                    new Text("Id", new Style(decoration: Decoration.Bold)),
                    new Text("Name", new Style(decoration: Decoration.Bold)),
                    new Text("Description", new Style(decoration: Decoration.Bold)),
                    new Text("Webhook Url", new Style(decoration: Decoration.Bold)),
                    new Text("Event Types", new Style(decoration: Decoration.Bold)),
                    new Text("Max Delivery Attempts", new Style(decoration: Decoration.Bold)),
                    new Text("Audience", new Style(decoration: Decoration.Bold))
                )
                .AddRow(
                    new Text(expectedSubscription.Id.ToString()),
                    new Text(expectedSubscription.Name),
                    new Text(expectedSubscription.Description),
                    new Text(expectedSubscription.WebhookUrl),
                    new Text(string.Join(", ", expectedSubscription.EventTypes.Select(et => et.Type))),
                    new Text(expectedSubscription.MaxDeliveryAttempts.ToString()),
                    new Text(expectedSubscription.Audience ?? string.Empty)
                ) :
            new Grid()
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
                );

        expectedConsole.Write(grid);
    }
}