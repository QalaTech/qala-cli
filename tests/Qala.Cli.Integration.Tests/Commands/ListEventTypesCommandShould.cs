using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListEventTypesCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, IBaseTestExecution
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("events list", true, null, null)]
    [InlineData("events ls", true, null, null)]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new ListEventTypesCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "list", null);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new ListEventTypesArgument();

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        throw new NotImplementedException();
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[green bold]Event Types:[/]");
        var grid = new Grid()
            .AddColumns(4)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Content Type", new Style(decoration: Decoration.Bold))
            );

        foreach (var eventType in fixture.AvailableEventTypes)
        {
            grid.AddRow(
                new Text(eventType.Id.ToString()),
                new Text(eventType.Type),
                new Text(eventType.Description),
                new Text(eventType.ContentType)
            );
        }
        expectedConsole.Write(grid);
    }
}