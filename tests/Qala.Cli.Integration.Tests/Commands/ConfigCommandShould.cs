using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class ConfigCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters()
    {
        // Arrange
        var console = new TestConsole();
        var command = new ConfigCommand(fixture.Mediator, console);
        var arguments = new List<string> { "config", "-k", fixture.ApiKey, "-e", fixture.AvailableEnvironments.First().Id.ToString() };
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("Processing request...", ctx => 
            {
                expectedOutput.MarkupLine($"[green bold]Configuration configured successfully:[/]");
                expectedOutput.Write(new Grid()
                    .AddColumns(2)
                    .AddRow(
                        new Text("Api Key", new Style(decoration: Decoration.Bold)),
                        new Text("Environment Id", new Style(decoration: Decoration.Bold))
                    )
                    .AddRow(
                        new Text(fixture.ApiKey),
                        new Text(fixture.AvailableEnvironments.First().Id.ToString())
                    ));
            });
        
        // Act
        var result = await command.ExecuteAsync(context, new ConfigArgument() { Key = fixture.ApiKey, EnvironmentId = fixture.AvailableEnvironments.First().Id });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i], actualLines[i]);
        }
    }
}
