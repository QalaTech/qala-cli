using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class GetSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<string>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("sources inspect TestSource", true, null, new string[] { "TestSource" })]
    [InlineData("sources i TestSource", true, null, new string[] { "TestSource" })]
    [InlineData("sources inspect", false, new string[] { "Source name is required." }, new string[] { "Source name is required" })]
    [InlineData("sources i", false, new string[] { "Source name is required." }, new string[] { "Source name is required" })]
    [InlineData("sources inspect NonExistingSource", false, null, new string[] { "Source not found" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new GetSourceCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "inspect", null);
        var name = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new GetSourceArgument()
        {
            Name = name
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public string ExtractArgumentsValues(List<string> arguments)
    {
        var nameIndex = arguments.IndexOf("inspect") != -1 ? arguments.IndexOf("inspect") : arguments.IndexOf("i");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            name = arguments[nameIndex + 1];
        }

        return name;
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Source retrieval:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedSource = fixture.AvailableSources.FirstOrDefault(sr => sr.Name == expectedOutput[0]);

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Source retrieved successfully:[/]");
        expectedConsole.Write(new Grid()
            .AddColumns(7)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Source Type", new Style(decoration: Decoration.Bold)),
                new Text("Http Methods", new Style(decoration: Decoration.Bold)),
                new Text("Authentication", new Style(decoration: Decoration.Bold)),
                new Text("Whitelisted IP Ranges", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedSource!.SourceId.ToString()),
                new Text(expectedSource.Name),
                new Text(expectedSource.Description),
                new Text(expectedSource.SourceType.ToString()),
                new Text(string.Join(", ", expectedSource.Configuration.AllowedHttpMethods)),
                new Text(expectedSource.Configuration.AuthenticationScheme!.Type.ToString()),
                new Text(string.Join(", ", expectedSource.Configuration.WhitelistedIpRanges))
            )
        );
    }
}