using Moq;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetEnvironmentCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, IBaseTestExecution
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("environment current", true, null, new string[] { "55908371-b629-40b5-97bc-d6064cf8d3cd" })]
    [InlineData("environment current", false, null, new string[] { "No environment was set" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        if (expectedOutput.Length > 0 && Guid.TryParse(expectedOutput[0], out _))
        {
            QalaCliBaseFixture.InitializeEnvironmentVariables(envId: expectedOutput[0]);
        }
        else
        {
            QalaCliBaseFixture.InitializeEnvironmentVariables();
        }

        var console = new TestConsole();
        var command = new GetEnvironmentCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "current", null);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        // Act
        var resultValidation = command.Validate(context, new GetEnvironmentArgument());
        var result = await command.ExecuteAsync(context, new GetEnvironmentArgument());

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);

    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Environment retrieval:[/]");
        expectedConsole.MarkupLine(expectedOutput[0]);
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var environment = string.IsNullOrWhiteSpace(expectedOutput[0]) ? null : fixture.AvailableEnvironments.FirstOrDefault(e => e.Id == Guid.Parse(expectedOutput[0]));
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[green bold]Environment retrieved successfully:[/]");
        if (environment != null)
        {
            expectedConsole.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("ID", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Region", new Style(decoration: Decoration.Bold)),
                new Text("Type", new Style(decoration: Decoration.Bold)),
                new Text("Schema Validation", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(environment.Id.ToString()),
                new Text(environment.Name),
                new Text(environment.Region),
                new Text(environment.EnvironmentType),
                new Text(environment.IsSchemaValidationEnabled ? "enabled" : "disabled")
            ));
        }
    }
}
