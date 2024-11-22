using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListEventTypesCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new ListEventTypesCommand(fixture.Mediator);
        var arguments = new List<string> { "events", "list" };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new ListEventTypesArgument());

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Event Types:", output);
        foreach (var eventType in fixture.AvailableEventTypes)
        {
            Assert.Contains(eventType.Type, output);
            Assert.Contains(eventType.Description, output);
            Assert.Contains(eventType.ContentType, output);
        }
    }
}
