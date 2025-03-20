using Moq;
using Qala.Cli.Commands.SubscriberGroups;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSubscriberGroupCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string SubscriberGroupName, string NewName, string Description, List<string> Topics, string Audience)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriber-group update TestSubscriberGroup -n TestSubscriberGroupUpdated", true, null, new string[] { "TestSubscriberGroup", "", "TestSubscriberGroupUpdated", "", "", "" })]
    [InlineData("sgp update TestSubscriberGroup2 --name TestSubscriberGroupUpdated2", true, null, new string[] { "TestSubscriberGroup2", "", "TestSubscriberGroupUpdated2", "", "", "" })]
    [InlineData("sgp update TestSubscriberGroup3 -n TestSubscriberGroupUpdated3 -d TestSubscriberGroupDescriptionUpdated", true, null, new string[] { "TestSubscriberGroup3", "", "TestSubscriberGroupUpdated3", "TestSubscriberGroupDescriptionUpdated", "", "" })]
    [InlineData("sgp update TestSubscriberGroup4 --name TestSubscriberGroupUpdated4 --description TestSubscriberGroupDescriptionUpdated", true, null, new string[] { "TestSubscriberGroup4", "", "TestSubscriberGroupUpdated4", "TestSubscriberGroupDescriptionUpdated", "", "" })]
    [InlineData("sgp update TestSubscriberGroup5 --name TestSubscriberGroupUpdated5 --description TestSubscriberGroupDescriptionUpdated --topics TestTopic1,TestTopic2", true, null, new string[] { "TestSubscriberGroup5", "", "TestSubscriberGroupUpdated5", "TestSubscriberGroupDescriptionUpdated", "TestTopic1,TestTopic2", "" })]
    [InlineData("sgp update TestSubscriberGroup6 --name TestSubscriberGroupUpdated6 --description TestSubscriberGroupDescriptionUpdated --topics TestTopic1,TestTopic2 --audience TestAudience", true, null, new string[] { "TestSubscriberGroup6", "", "TestSubscriberGroupUpdated6", "TestSubscriberGroupDescriptionUpdated", "TestTopic1,TestTopic2", "TestAudience" })]
    [InlineData("sgp update -n TestSubscriberGroupUpdated7 -d TestSubscriberGroupDescriptionUpdated", false, new string[] { "Subscriber Group name is required." }, new string[] { "Subscriber Group name is required" })]
    [InlineData("sgp update --name TestSubscriberGroupUpdated8 --description TestSubscriberGroupDescriptionUpdated", false, new string[] { "Subscriber Group name is required." }, new string[] { "Subscriber Group name is required" })]
    [InlineData("sgp update NonExistingSubscriberGroup -n TestSubscriberGroupUpdated9 -d TestSubscriberGroupDescriptionUpdated", false, null, new string[] { "Subscriber Group not found" })]
    [InlineData("sgp update NonExistingSubscriberGroup --name TestSubscriberGroupUpdated10 --description TestSubscriberGroupDescriptionUpdated", false, null, new string[] { "Subscriber Group not found" })]
    [InlineData("sgp update TestSubscriberGroup11 --name TestSubscriberGroupUpdated11 --description TestSubscriberGroupDescriptionUpdated --topics TestTopic1,TestTopic2 --audience ", true, null, new string[] { "TestSubscriberGroup11", "", "TestSubscriberGroupUpdated11", "TestSubscriberGroupDescriptionUpdated", "TestTopic1,TestTopic2", "" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new UpdateSubscriberGroupCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var (SubscriberGroupName, NewName, Description, Topics, Audience) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new UpdateSubscriberGroupArgument()
        {
            Name = SubscriberGroupName,
            NewName = NewName,
            Description = Description,
            Topics = Topics,
            Audience = Audience
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string SubscriberGroupName, string NewName, string Description, List<string> Topics, string Audience) ExtractArgumentsValues(List<string> arguments)
    {
        var nameIndex = arguments.IndexOf("update");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count && !arguments[nameIndex + 1].StartsWith("-"))
        {
            name = arguments[nameIndex + 1];
        }

        var newNameIndex = arguments.IndexOf("-n") != -1 ? arguments.IndexOf("-n") : arguments.IndexOf("--name");
        var newName = string.Empty;
        if (newNameIndex != -1 && newNameIndex + 1 < arguments.Count)
        {
            newName = arguments[newNameIndex + 1];
        }

        var descriptionIndex = arguments.IndexOf("-d") != -1 ? arguments.IndexOf("-d") : arguments.IndexOf("--description");
        var description = string.Empty;
        if (descriptionIndex != -1 && descriptionIndex + 1 < arguments.Count)
        {
            description = arguments[descriptionIndex + 1];
        }

        var topicsIndex = arguments.IndexOf("-t") != -1 ? arguments.IndexOf("-t") : arguments.IndexOf("--topics");
        var topics = new List<string>();
        if (topicsIndex != -1 && topicsIndex + 1 < arguments.Count)
        {
            topics = arguments[topicsIndex + 1].Split(',').ToList();
        }

        var audienceIndex = arguments.IndexOf("-a") != -1 ? arguments.IndexOf("-a") : arguments.IndexOf("--audience");
        var audience = string.Empty;
        if (audienceIndex != -1 && audienceIndex + 1 < arguments.Count)
        {
            audience = arguments[audienceIndex + 1];
        }

        return (name, newName, description, topics, audience);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Subscriber Group update:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var currentSubscriberGroup = fixture.AvailableSubscriberGroups.FirstOrDefault(t => t.Name == expectedOutput[0]);

        var expectedSubscriberGroup = new SubscriberGroupPrincipal()
        {
            Key = string.IsNullOrWhiteSpace(expectedOutput[1]) ? currentSubscriberGroup.Key : Guid.Parse(expectedOutput[1]),
            Name = string.IsNullOrEmpty(expectedOutput[2]) ? currentSubscriberGroup.Name : expectedOutput[2],
            Description = string.IsNullOrEmpty(expectedOutput[3]) ? currentSubscriberGroup.Description : expectedOutput[3],
            AvailablePermissions = string.IsNullOrEmpty(expectedOutput[4]) ? currentSubscriberGroup.AvailablePermissions : expectedOutput[4].Split(',').Select(p => new Permission() { ResourceId = p, PermissionType = "Topic:Subscribe", ResourceType = "Topic", }).ToList(),
            Audience = expectedOutput[5] == null ? currentSubscriberGroup.Audience : expectedOutput[5]
        };

        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Subscriber Group updated successfully.[/]");
        var grid = new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Key", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Topics", new Style(decoration: Decoration.Bold)),
                new Text("Audience", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedSubscriberGroup.Key.ToString()),
                new Text(expectedSubscriberGroup.Name),
                new Text(expectedSubscriberGroup.Description),
                new Text(string.Join(", ", expectedSubscriberGroup.AvailablePermissions.Select(p => p.ResourceId))),
                new Text(expectedSubscriberGroup.Audience)
            );

        expectedConsole.Write(grid);
    }
}