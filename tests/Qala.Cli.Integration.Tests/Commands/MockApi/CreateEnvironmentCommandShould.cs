using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands.MockApi;

public class CreateEnvironmentCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region -t newly-env-type", "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestEnv", "newly-region", "newly-env-type", "True")]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region -t newly-env-type --disableSchemaValidation", "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestEnv", "newly-region", "newly-env-type", "False")]
    public async Task Execute_WithValidParameters(string input, string expectedId, string expectedName, string expectedRegion, string expectedType, string expectedSchemaValidation)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new CreateEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var envName = arguments[3];
        var envRegion = arguments[5];
        var envType = arguments[7];
        var envDisableSchemaValidation = arguments.Contains("--disableSchemaValidation");
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine($"[green]Environment created successfully:[/]");
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("ID", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Region", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Schema Validation Enabled", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedId),
                new Text(expectedName),
                new Text(expectedRegion),
                new Text(expectedType),
                new Text(expectedSchemaValidation)
            )
        );

        // Act
        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Name = envName, Region = envRegion, Type = envType, DisableSchemaValidation = envDisableSchemaValidation });

        // Assert
        Assert.Equal(0, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }

    [Theory]
    [InlineData("environment create -r newly-region -t newly-env-type", "Name is required")]
    [InlineData("environment create -n NewlyCreatedTestEnv -t newly-env-type", "Region is required")]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region", "Type is required")]
    public async Task Execute_WithInvalidParameters(string input, string expectedErrorMessage)
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.MarkupLine("Processing request...");
        expectedOutput.MarkupLine($"[red bold]Error during Environment creation:[/]");
        expectedOutput.MarkupLine($"[red]{expectedErrorMessage}[/]");

        // Act
        var nameIndex = arguments.IndexOf("--name") != -1 ? arguments.IndexOf("--name") : arguments.IndexOf("-n");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            name = arguments[nameIndex + 1];
        }

        var regionIndex = arguments.IndexOf("--region") != -1 ? arguments.IndexOf("--region") : arguments.IndexOf("-r");
        var region = string.Empty;
        if (regionIndex != -1 && regionIndex + 1 < arguments.Count)
        {
            region = arguments[regionIndex + 1];
        }

        var typeIndex = arguments.IndexOf("--type") != -1 ? arguments.IndexOf("--type") : arguments.IndexOf("-t");
        var type = string.Empty;
        if (typeIndex != -1 && typeIndex + 1 < arguments.Count)
        {
            type = arguments[typeIndex + 1];
        }

        var result = await command.ExecuteAsync(context, new CreateEnvironmentArgument() { Name = name, Region = region, Type = type });

        // Assert
        Assert.Equal(-1, result);
        var expectedLines = expectedOutput.Lines;
        var actualLines = console.Lines;

        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Contains(expectedLines[i], actualLines);
        }
    }
}
