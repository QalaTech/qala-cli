using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string Description, List<string> EventTypeNames)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("topic create --name NewlyCreatedTestTopic --description newly-topic-description --events TestEvent1,TestEvent2", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestTopic", "newly-topic-description", "Provisioning", "TestEvent1,TestEvent2" })]
    [InlineData("topic create -n NewlyCreatedTestTopic -d newly-topic-description -e TestEvent1,TestEvent2", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestTopic", "newly-topic-description", "Provisioning", "TestEvent1,TestEvent2" })]
    [InlineData("topic create -n NewlyCreatedTestTopic -d newly-topic-description", false, new string[] { "At least one event type name is required." }, new string[] { "Event type names are required" })]
    [InlineData("topic create -n NewlyCreatedTestTopic -e TestEvent1,TestEvent2", false, new string[] { "Topic description is required." }, new string[] { "Description is required" })]
    [InlineData("topic create -d newly-topic-description -e TestEvent1,TestEvent2", false, new string[] { "Topic name is required." }, new string[] { "Name is required" })]
    [InlineData("topic create -n NewlyCreatedTestTopic -d newly-topic-description -e NonExistingTestEvent1,NonExistingTestEvent2", false, null, new string[] { "Event types not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateTopicCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var (name, description, eventTypeNames) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new CreateTopicArgument()
        {
            Name = name,
            Description = description,
            EventTypeNames = eventTypeNames
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string Name, string Description, List<string> EventTypeNames) ExtractArgumentsValues(List<string> arguments)
    {
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

        var eventTypeIndex = arguments.IndexOf("--events") != -1 ? arguments.IndexOf("--events") : arguments.IndexOf("-e");
        var eventTypeNames = new List<string>();
        if (eventTypeIndex != -1 && eventTypeIndex + 1 < arguments.Count)
        {
            eventTypeNames = arguments[eventTypeIndex + 1].Split(',').ToList();
        }

        return (name, description, eventTypeNames);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Topic creation:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedTopic = new Topic()
        {
            Id = Guid.Parse(expectedOutput[0]),
            Name = expectedOutput[1],
            Description = expectedOutput[2],
            ProvisioningState = expectedOutput[3],
            EventTypes = expectedOutput[4].Split(',').Select(et => new EventType() { Type = et }).ToList()
        };

        expectedConsole.MarkupLine($"[green]Topic created successfully:[/]");
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                new Text("Event Types", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedTopic.Id.ToString()),
                new Text(expectedTopic.Name),
                new Text(expectedTopic.Description),
                new Text(expectedTopic.ProvisioningState),
                new Text(string.Join(", ", expectedTopic.EventTypes.Select(et => et.Type)))
            )
        );
    }
}