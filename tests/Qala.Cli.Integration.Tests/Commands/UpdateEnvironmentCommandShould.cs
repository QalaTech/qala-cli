using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateEnvironmentCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<bool>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment update --disableSchemaValidation", true, null, new string[] { "55908371-b629-40b5-97bc-d6064cf8d3cd" })]
    [InlineData("env update --disableSchemaValidation", true, null, new string[] { "55908371-b629-40b5-97bc-d6064cf8d3cd" })]
    [InlineData("environment update --disableSchemaValidation", false, null, new string[] { "No environment was set found" })]
    [InlineData("env update --disableSchemaValidation", false, null, new string[] { "No environment was set found" })]
    [InlineData("environment update --disableSchemaValidation", false, null, new string[] { "Environment not found" })]
    [InlineData("env update --disableSchemaValidation", false, null, new string[] { "Environment not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        QalaCliBaseFixture.InitializeEnvironmentVariables();
        if (Guid.TryParse(expectedOutput[0], out var envId))
        {
            QalaCliBaseFixture.InitializeEnvironmentVariables(envId: expectedOutput[0]);
        }
        else if (expectedOutput[0] == "Environment not found")
        {
            QalaCliBaseFixture.InitializeEnvironmentVariables(envId: Guid.Empty.ToString());
        }

        var console = new TestConsole();
        var command = new UpdateEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var disableSchemaValidation = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new UpdateEnvironmentArgument()
        {
            DisableSchemaValidation = disableSchemaValidation
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public bool ExtractArgumentsValues(List<string> arguments)
    {
        return arguments.Contains("--disableSchemaValidation");
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Environment update:[/]");
        expectedConsole.MarkupLine(expectedOutput[0]);
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedEnvironment = fixture.AvailableEnvironments.FirstOrDefault(e => e.Id == Guid.Parse(expectedOutput[0]));

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Environment updated successfully:[/]");
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
                new Text(expectedEnvironment!.Id.ToString()),
                new Text(expectedEnvironment.Name),
                new Text(expectedEnvironment.Region),
                new Text(expectedEnvironment.EnvironmentType),
                new Text(expectedEnvironment.IsSchemaValidationEnabled ? "True" : "False")
            )
        );
    }
}