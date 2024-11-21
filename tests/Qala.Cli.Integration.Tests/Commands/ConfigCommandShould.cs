using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class ConfigCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var command = new ConfigCommand(fixture.Mediator);
        var arguments = new List<string> { "config", "-k", fixture.ApiKey, "-e", fixture.AvailableEnvironments.First().Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new ConfigArgument() { Key = fixture.ApiKey, EnvironmentId = fixture.AvailableEnvironments.First().Id });

        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Api Key", output);
        Assert.Contains("Environment Id", output);
        Assert.Contains(fixture.ApiKey, output);
        Assert.Contains(fixture.AvailableEnvironments.First().Id.ToString(), output);
        var key = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);
        var environmentId = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User);
        Assert.Equal(fixture.ApiKey, key);
        Assert.Equal(fixture.AvailableEnvironments.First().Id.ToString(), environmentId);
    }
}
