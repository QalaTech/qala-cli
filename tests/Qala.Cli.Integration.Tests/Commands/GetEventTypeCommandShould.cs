using Moq;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEventTypeCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new GetEventTypeCommand(fixture.Mediator);
        var arguments = new List<string> { "events", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument() { Id = fixture.AvailableEventTypes.First().Id });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        var eventType = fixture.AvailableEventTypes.First();
        Assert.Contains("Event Type retrieved successfully:", output);
        Assert.Contains(eventType.Type, output);
        Assert.Contains(eventType.ContentType, output);
        Assert.Contains(string.Join(", ", eventType.Categories), output);
        Assert.Contains("Schema", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new GetEventTypeCommand(fixture.Mediator);
        var arguments = new List<string> { "events", "inspect" };
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetEventTypeArgument(){ Id = Guid.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Event Type retrieval:", output);
    }
}
