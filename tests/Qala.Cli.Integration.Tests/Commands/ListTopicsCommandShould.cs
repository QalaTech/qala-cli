using Moq;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class ListTopicsCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new ListTopicsCommand(fixture.Mediator);
        var arguments = new List<string> { "topics", "list" };
        var context = new CommandContext(arguments, _remainingArguments, "list", null);
        AnsiConsole.Record();

        // Act
        var result = await command.ExecuteAsync(context, new ListTopicsArgument());

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Topics:", output);
        foreach (var topic in fixture.AvailableTopics)
        {
            Assert.Contains(topic.Name, output);
            Assert.Contains(topic.ProvisioningState, output);
        }
    }
}
