using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<string>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("topic inspect TestTopic", true, null, new string[] { "TestTopic" })]
    [InlineData("topic i TestTopic", true, null, new string[] { "TestTopic" })]
    [InlineData("topic inspect", false, new string[] { "Topic name is required." }, new string[] { "Topic name is required" })]
    [InlineData("topic i", false, new string[] { "Topic name is required." }, new string[] { "Topic name is required" })]
    [InlineData("topic inspect NonExistingTopic", false, null, new string[] { "Topic not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetTopicCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        var name = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new GetTopicArgument()
        {
            Name = name
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public string ExtractArgumentsValues(List<string> arguments)
    {
        var nameIndex = arguments.IndexOf("inspect") != -1 ? arguments.IndexOf("inspect") : arguments.IndexOf("i");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            name = arguments[nameIndex + 1];
        }

        return name;
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Topic retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var topic = fixture.AvailableTopics.FirstOrDefault(t => t.Name == expectedOutput[0]);
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
                new Text(topic.Id.ToString()),
                new Text(topic.Name),
                new Text(topic.Description),
                new Text(topic.ProvisioningState),
                new Text(string.Join(", ", topic.EventTypes.Select(et => et.Type)))
            )
        );
    }
}