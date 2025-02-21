using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string Description, List<string> EventTypeNames)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("topics update TestTopic -d TestTopicDescriptionUpdated", true, null, new string[] { "TestTopic", "", "", "TestTopicDescriptionUpdated", "" })]
    [InlineData("topics update TestTopic2 --description TestTopicDescriptionUpdated", true, null, new string[] { "TestTopic2", "", "", "TestTopicDescriptionUpdated", "" })]
    [InlineData("topics update TestTopic3 -d TestTopicDescriptionUpdated -e TestEvent1,TestEvent2", true, null, new string[] { "TestTopic3", "", "", "TestTopicDescriptionUpdated", "TestEvent1,TestEvent2" })]
    [InlineData("topics update TestTopic4 --description TestTopicDescriptionUpdated --events TestEvent1,TestEvent2", true, null, new string[] { "TestTopic4", "", "", "TestTopicDescriptionUpdated", "TestEvent1,TestEvent2" })]
    [InlineData("topics update -d TestTopicDescriptionUpdated -e TestEvent1,TestEvent2", false, new string[] { "Topic name is required." }, new string[] { "Topic name is required" })]
    [InlineData("topics update --description TestTopicDescriptionUpdated --events TestEvent1,TestEvent2", false, new string[] { "Topic name is required." }, new string[] { "Topic name is required" })]
    [InlineData("topics update NonExistingTopic -d TestTopicDescriptionUpdated -e TestEvent1,TestEvent2", false, null, new string[] { "Topic not found" })]
    [InlineData("topics update NonExistingTopic --description TestTopicDescriptionUpdated --events TestEvent1,TestEvent2", false, null, new string[] { "Topic not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new UpdateTopicCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var (Name, Description, EventTypeNames) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new UpdateTopicArgument()
        {
            Name = Name,
            Description = Description,
            EventTypeNames = EventTypeNames
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
        var nameIndex = arguments.IndexOf("update");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count && !arguments[nameIndex + 1].StartsWith("-"))
        {
            name = arguments[nameIndex + 1];
        }

        var descriptionIndex = arguments.IndexOf("-d") != -1 ? arguments.IndexOf("-d") : arguments.IndexOf("--description");
        var description = string.Empty;
        if (descriptionIndex != -1 && descriptionIndex + 1 < arguments.Count)
        {
            description = arguments[descriptionIndex + 1];
        }

        var eventsIndex = arguments.IndexOf("-e") != -1 ? arguments.IndexOf("-e") : arguments.IndexOf("--events");
        var events = new List<string>();
        if (eventsIndex != -1 && eventsIndex + 1 < arguments.Count)
        {
            events = arguments[eventsIndex + 1].Split(',').ToList();
        }

        return (name, description, events);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Topic update:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var currentTopic = fixture.AvailableTopics.FirstOrDefault(t => t.Name == expectedOutput[0]);

        var expectedTopic = new Topic()
        {
            Id = string.IsNullOrWhiteSpace(expectedOutput[1]) ? currentTopic.Id : Guid.Parse(expectedOutput[1]),
            Name = string.IsNullOrWhiteSpace(expectedOutput[2]) ? currentTopic.Name : expectedOutput[2],
            Description = string.IsNullOrWhiteSpace(expectedOutput[3]) ? currentTopic.Description : expectedOutput[3],
            ProvisioningState = currentTopic.ProvisioningState,
            EventTypes = string.IsNullOrWhiteSpace(expectedOutput[4]) ? currentTopic.EventTypes : expectedOutput[4].Split(',').Select(et => new EventType() { Type = et }).ToList()
        };

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Topic updated successfully.[/]");
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