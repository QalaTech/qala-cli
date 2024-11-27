using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateTopicCommandShould(QalaCliBaseFixture fixture): IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var topicName = fixture.AvailableTopics.First().Name;
        var topicDescription = "updated";
        var eventTypeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new UpdateTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topic", "update", topicName, "-d", topicDescription, "-e", string.Join(",", eventTypeIds) };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new UpdateTopicArgument() { Name = topicName, Description = topicDescription, EventTypeIds = eventTypeIds });
        
        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Topic updated successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new UpdateTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topic", "update" };
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new UpdateTopicArgument());
        
        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Topic update:", output);
    }
}