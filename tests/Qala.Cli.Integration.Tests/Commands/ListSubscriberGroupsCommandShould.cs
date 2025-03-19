using Moq;
using Qala.Cli.Commands.SubscriberGroups;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListSubscriberGroupsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, IBaseTestExecution
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriber-groups list", true, null, null)]
    [InlineData("subscriber-groups ls", true, null, null)]
    [InlineData("sgp list", true, null, null)]
    [InlineData("sgp ls", true, null, null)]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new ListSubscriberGroupsCommand(fixture.Mediator, console);
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

        var inputArguments = new ListSubscriberGroupsArgument();

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
        expectedConsole.MarkupLine("[green bold]Subscriber Groups:[/]");
        var grid = new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Topics", new Style(decoration: Decoration.Bold)),
                new Text("Audience", new Style(decoration: Decoration.Bold))
            );

        foreach (var subscriberGroup in fixture.AvailableSubscriberGroups)
        {
            grid.AddRow(
                new Text(subscriberGroup.Id.ToString()),
                new Text(subscriberGroup.Name),
                new Text(subscriberGroup.Description),
                new Text(string.Join(", ", subscriberGroup.AvailablePermissions.Select(p => p.ResourceId))),
                new Text(subscriberGroup.Audience)
            );
        }

        expectedConsole.Write(grid);
    }
}