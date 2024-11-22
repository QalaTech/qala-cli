using System;
using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateEnvironmetCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var envName = "NewlyCreatedTestEnv";
        var envRegion = "newly-region";
        var envType = "newly-env-type";
        var command = new CreateEnvironmentCommand(fixture.Mediator);
        var arguments = new List<string> { "environment", "create", "-n", envName, "-r", envRegion, "-t", envType };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Name = envName, Region = envRegion, Type = envType });
        
        // Assert
        Assert.Equal(0, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Environment created successfully", output);
        Assert.Contains(envName, output);
        Assert.Contains(envRegion, output);
        Assert.Contains(envType, output);
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var command = new CreateEnvironmentCommand(fixture.Mediator);
        var arguments = new List<string> { "environment", "create", "-r", "newly-region", "-t", "newly-env-type" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        AnsiConsole.Record();
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Region = "newly-region", Type = "newly-env-type" });
        
        // Assert
        Assert.Equal(-1, result);
        var output = AnsiConsole.ExportText();
        Assert.Contains("Error creating environment", output);
    }
}
