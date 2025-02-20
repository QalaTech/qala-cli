using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEventTypeCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<string>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("events inspect TestEvent1", true, null, new string[] { "TestEvent1" })]
    [InlineData("events i TestEvent1", true, null, new string[] { "TestEvent1" })]
    [InlineData("events inspect", false, new string[] { "Event type Name is required" }, new string[] { "Event Type name is required" })]
    [InlineData("events i", false, new string[] { "Event type Name is required" }, new string[] { "Event Type name is required" })]
    [InlineData("events inspect NonExisting", false, null, new string[] { "Event type not found" })]
    [InlineData("events i NonExisting", false, null, new string[] { "Event type not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables(fixture.AvailableApiKeys.First(), fixture.AvailableEnvironments.First().Id.ToString());
        var console = new TestConsole();
        var command = new GetEventTypeCommand(fixture.Mediator, console);
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

        var inputArguments = new GetEventTypeArgument()
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
        expectedConsole.MarkupLine($"[red bold]Error during Event Type retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedEventType = fixture.AvailableEventTypes.FirstOrDefault(ev => ev.Type == expectedOutput[0]);

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Event Type retrieved successfully:[/]");
        expectedConsole.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Content Type", new Style(decoration: Decoration.Bold)),
                new Text("Categories", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedEventType!.Id.ToString()),
                new Text(expectedEventType.Type.ToString()),
                new Text(expectedEventType.Description),
                new Text(expectedEventType.ContentType),
                new Text(string.Join(", ", expectedEventType.Categories))
            )
        );
    }
}