using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class ListTopicsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("topics list")]
    [InlineData("topics ls")]
    public async Task Execute_WithValidParameters(string input)
    {
        // Arrange
        var console = new TestConsole();
        var command = new ListTopicsCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var listIndex = arguments.IndexOf("list") != -1 ? arguments.IndexOf("list") : arguments.IndexOf("ls");
        var context = new CommandContext(arguments, _remainingArguments, arguments[listIndex], null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine("[green bold]Topics:[/]");
        var grid = new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Provisioning State", new Style(decoration: Decoration.Bold)),
                new Text("Event Types", new Style(decoration: Decoration.Bold))
            );

        foreach (var topic in fixture.AvailableTopics)
        {
            grid.AddRow(
                new Text(topic.Id.ToString()),
                new Text(topic.Name),
                new Text(topic.Description),
                new Text(topic.ProvisioningState),
                new Text(string.Join(", ", topic.EventTypes.Select(et => et.Type)))
            );
        }

        expectedOutput.Write(grid);

        // Act
        var result = await command.ExecuteAsync(context, new ListTopicsArgument());

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }
}