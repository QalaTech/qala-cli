using Moq;
using Qala.Cli.Commands.SubscriberGroups;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSubscriberGroupCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<string>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriber-group inspect TestSubscriberGroup", true, null, new string[] { "TestSubscriberGroup" })]
    [InlineData("sgp inspect TestSubscriberGroup", true, null, new string[] { "TestSubscriberGroup" })]
    [InlineData("subscriber-group inspect", false, new string[] { "Subscriber Group name is required." }, new string[] { "Subscriber Group name is required" })]
    [InlineData("sgp inspect", false, new string[] { "Subscriber Group name is required." }, new string[] { "Subscriber Group name is required" })]
    [InlineData("subscriber-group inspect NonExistingSubscriberGroup", false, null, new string[] { "Subscriber Group not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetSubscriberGroupCommand(fixture.Mediator, console);
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

        var inputArguments = new GetSubscriberGroupArgument()
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
        expectedConsole.MarkupLine($"[red bold]Error during Subscriber Group retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var subscriberGroup = fixture.AvailableSubscriberGroups.FirstOrDefault(x => x.Name == expectedOutput[0]);
        expectedConsole.MarkupLine("Processing request...");
        var grid = new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Topics", new Style(decoration: Decoration.Bold)),
                new Text("Audience", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(subscriberGroup.Id.ToString()),
                new Text(subscriberGroup.Name),
                new Text(subscriberGroup.Description),
                new Text(string.Join(", ", subscriberGroup.AvailablePermissions.Select(p => p.ResourceId))),
                new Text(subscriberGroup.Audience)
            );
        expectedConsole.Write(grid);
    }
}