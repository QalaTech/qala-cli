using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateEnvironmentCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string Region, string Type, bool DisableSchemaValidation)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region -t newly-env-type", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestEnv", "newly-region", "newly-env-type", "True" })]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region -t newly-env-type --disableSchemaValidation", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestEnv", "newly-region", "newly-env-type", "False" })]
    [InlineData("environment create -r newly-region -t newly-env-type", false, new string[] { "Name is required" }, new string[] { "Name is required" })]
    [InlineData("environment create -n NewlyCreatedTestEnv -t newly-env-type", false, new string[] { "Region is required" }, new string[] { "Region is required" })]
    [InlineData("environment create -n NewlyCreatedTestEnv -r newly-region", false, new string[] { "Type is required" }, new string[] { "Type is required" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        var console = new TestConsole();
        var command = new CreateEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "config", null);
        var (name, region, type, disableSchemaValidation) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new CreateEnvironmentArgument()
        {
            Name = name,
            Region = region,
            Type = type,
            DisableSchemaValidation = disableSchemaValidation
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string Name, string Region, string Type, bool DisableSchemaValidation) ExtractArgumentsValues(List<string> arguments)
    {
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

        var disableSchemaValidationIndex = arguments.IndexOf("--disableSchemaValidation");
        var disableSchemaValidation = disableSchemaValidationIndex != -1;

        return (name, region, type, disableSchemaValidation);
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine($"[green]Environment created successfully:[/]");
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("ID", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Region", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Schema Validation Enabled", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedOutput[0]),
                new Text(expectedOutput[1]),
                new Text(expectedOutput[2]),
                new Text(expectedOutput[3]),
                new Text(expectedOutput[4])
            )
        );
    }
}