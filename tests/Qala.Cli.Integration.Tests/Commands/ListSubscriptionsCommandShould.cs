using Moq;
using Qala.Cli.Commands.Subscriptions;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListSubscriptionsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new ListSubscriptionsCommand(fixture.Mediator);
        var topicName = fixture.AvailableSubscriptions.First().TopicName;
        var arguments = new List<string> { "subscriptions", "list", topicName };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new ListSubscriptionsArgument() { TopicName = topicName});

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Subscriptions:", output);
    }
}