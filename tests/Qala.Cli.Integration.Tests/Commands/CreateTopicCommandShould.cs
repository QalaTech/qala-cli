using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var topicName = "NewlyCreatedTestTopic";
        var topicDescription = "newly-created-topic-description";
        var eventTypeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new CreateTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topic", "create", "-n", topicName, "-d", topicDescription, "-e", string.Join(",", eventTypeIds) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateTopicArgument() { Name = topicName, Description = topicDescription, EventTypeIds = eventTypeIds });
        
        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Topic created successfully:", output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new CreateTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topic", "create" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateTopicArgument());
        
        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during Topic creation:", output);
    }
}