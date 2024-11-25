using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetTopicCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new GetTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topics", "name" };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetTopicArgument() { Name = fixture.AvailableTopics.First().Name });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        var topic = fixture.AvailableTopics.First();
        Assert.Contains("Topic retrieved:", output);
        Assert.Contains(topic.Name, output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new GetTopicCommand(fixture.Mediator);
        var arguments = new List<string> { "topics", "name" };
        var context = new CommandContext(arguments, _remainingArguments, "name", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new GetTopicArgument(){ Name = string.Empty });

        // Assert
        Assert.NotEqual(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error during topic retrieval:", output);
    }
}