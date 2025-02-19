using System.IO.Compression;
using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class GetEventTypeCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("events inspect TestEvent1")]
    [InlineData("events i TestEvent1")]
    public async Task Execute_WithValidParameters(string input)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables(fixture.AvailableApiKeys.First(), fixture.AvailableEnvironments.First().Id.ToString());
        var console = new TestConsole();
        var command = new GetEventTypeCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var inspectIndex = arguments.IndexOf("inspect") != -1 ? arguments.IndexOf("inspect") : arguments.IndexOf("i");
        var context = new CommandContext(arguments, _remainingArguments, arguments[inspectIndex], null);
        var expectedEventType = fixture.AvailableEventTypes.FirstOrDefault(ev => ev.Type == "TestEvent1");
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine($"[green bold]Event Type retrieved successfully:[/]");
        expectedOutput.Write(new Grid()
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

        if (!string.IsNullOrEmpty(expectedEventType.Schema))
        {
            expectedOutput.Write(
                new Panel(new JsonText(expectedEventType.Schema))
                    .Header("Schema")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow)
            );
        }

        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument()
        {
            Name = "TestEvent1"
        });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }

    [Theory]
    [InlineData("events inspect", "Event Type name is required")]
    [InlineData("events i", "Event Type name is required")]
    [InlineData("events inspect NonExisting", "Event type not found")]
    [InlineData("events i NonExisting", "Event type not found")]
    public async Task Execute_WithInvalidParameters(string input, string expectedErrorMessage)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables(fixture.AvailableApiKeys.First(), fixture.AvailableEnvironments.First().Id.ToString());
        var console = new TestConsole();
        var command = new GetEventTypeCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var inspectIndex = arguments.IndexOf("inspect") != -1 ? arguments.IndexOf("inspect") : arguments.IndexOf("i");
        var context = new CommandContext(arguments, _remainingArguments, arguments[inspectIndex], null);
        var eventTypeInput = arguments.Count > 2 ? arguments[2] : string.Empty;
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine($"[red bold]Error during Event Type retrieval:[/]");
        expectedOutput.MarkupLine($"[red]{expectedErrorMessage}[/]");

        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument()
        {
            Name = eventTypeInput
        });

        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }
}