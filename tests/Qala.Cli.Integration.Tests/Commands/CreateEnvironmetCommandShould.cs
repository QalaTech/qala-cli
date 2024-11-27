using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateEnvironmetCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;
    
    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var envName = "NewlyCreatedTestEnv";
        var envRegion = "newly-region";
        var envType = "newly-env-type";
        var command = new CreateEnvironmentCommand(fixture.Mediator, console);
        var arguments = new List<string> { "environment", "create", "-n", envName, "-r", envRegion, "-t", envType };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine($"[green]Environment created successfully:[/]");
            expectedOutput.Status()
                .AutoRefresh(true)
                .Spinner(Spinner.Known.Star2)
                .SpinnerStyle(Style.Parse("yellow bold"))
                .Start("Processing request...", ctx => 
                {
            expectedOutput.Write(new Grid()
                .AddColumns(4)
                .AddRow(
                    new Text("ID", new Style(decoration: Decoration.Bold)),
                    new Text("Name", new Style(decoration: Decoration.Bold)),
                    new Text("Region", new Style(decoration: Decoration.Bold)),
                    new Text("Type", new Style(decoration: Decoration.Bold))
                )
                .AddRow(
                    new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                    new Text("NewlyCreatedTestEnv"),
                    new Text("newly-region"),
                    new Text("newly-env-type")
                )
            );
        });
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Name = envName, Region = envRegion, Type = envType });
        
        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }

    [Fact]
    public async Task Execute_WithInvalidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateEnvironmentCommand(fixture.Mediator, console);
        var arguments = new List<string> { "environment", "create", "-r", "newly-region", "-t", "newly-env-type" };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("Processing request...", ctx => 
            {
                expectedOutput.MarkupLine($"[red bold]Error during Environment creation:[/]");
                expectedOutput.MarkupLine($"[red]Name is required[/]");
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Region = "newly-region", Type = "newly-env-type" });
        
        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}
