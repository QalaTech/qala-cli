using Moq;
using Qala.Cli.Commands.SubscriberGroups;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSubscriberGroupCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string Description, List<string> Topics, string Audience)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("subscriber-group create --name NewlyCreatedTestSubscriberGroup --description newly-subscriber-group-description --topics TestTopic1,TestTopic2 --audience TestAudience", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscriberGroup", "newly-subscriber-group-description", "TestTopic1,TestTopic2", "TestAudience" })]
    [InlineData("sgp create -n NewlyCreatedTestSubscriberGroup -d newly-subscriber-group-description -t TestTopic1,TestTopic2 -a TestAudience", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscriberGroup", "newly-subscriber-group-description", "TestTopic1,TestTopic2", "TestAudience" })]
    [InlineData("sgp create -n NewlyCreatedTestSubscriberGroup -d newly-subscriber-group-description -t TestTopic1,TestTopic2", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "NewlyCreatedTestSubscriberGroup", "newly-subscriber-group-description", "TestTopic1,TestTopic2", "" })]
    [InlineData("sgp create -d newly-subscriber-group-description -t TestTopic1,TestTopic2 -a TestAudience", false, new string[] { "Subscriber Group name is required." }, new string[] { "Subscriber Group name is required" })]
    [InlineData("sgp create -n NewlyCreatedTestSubscriberGroup -t TestTopic1,TestTopic2 -a TestAudience", false, new string[] { "Subscriber Group description is required." }, new string[] { "Subscriber Group description is required" })]
    [InlineData("sgp create -n NewlyCreatedTestSubscriberGroup -d newly-subscriber-group-description -a TestAudience", false, new string[] { "At least one topic is required." }, new string[] { "At least one topic is required" })]
    [InlineData("sgp create -n NewlyCreatedTestSubscriberGroup -d newly-subscriber-group-description -t TestTopic1,TestTopic2 -a Wrong_Audience", false, new string[] { "Audience must only contain alphanumerical values (A-Z, a-z, 0-9)." }, new string[] { "Audience must only contain alphanumerical values (A-Z, a-z, 0-9)" })]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateSubscriberGroupCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var (name, description, topics, audience) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new CreateSubscriberGroupArgument()
        {
            Name = name,
            Description = description,
            Topics = topics,
            Audience = audience
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = await command.ExecuteAsync(context, inputArguments);

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string Name, string Description, List<string> Topics, string Audience) ExtractArgumentsValues(List<string> arguments)
    {
        var nameIndex = arguments.IndexOf("--name") != -1 ? arguments.IndexOf("--name") : arguments.IndexOf("-n");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            name = arguments[nameIndex + 1];
        }

        var descriptionIndex = arguments.IndexOf("--description") != -1 ? arguments.IndexOf("--description") : arguments.IndexOf("-d");
        var description = string.Empty;
        if (descriptionIndex != -1 && descriptionIndex + 1 < arguments.Count)
        {
            description = arguments[descriptionIndex + 1];
        }

        var topicsIndex = arguments.IndexOf("--topics") != -1 ? arguments.IndexOf("--topics") : arguments.IndexOf("-t");
        var topics = new List<string>();
        if (topicsIndex != -1 && topicsIndex + 1 < arguments.Count)
        {
            topics = arguments[topicsIndex + 1].Split(',').ToList();
        }

        var audienceIndex = arguments.IndexOf("--audience") != -1 ? arguments.IndexOf("--audience") : arguments.IndexOf("-a");
        var audience = string.Empty;
        if (audienceIndex != -1 && audienceIndex + 1 < arguments.Count)
        {
            audience = arguments[audienceIndex + 1];
        }

        return (name, description, topics, audience);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Subscriber Group creation:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedPermissions = new List<Permission>();
        foreach (var topic in expectedOutput[3].Split(','))
        {
            expectedPermissions.Add(new Permission()
            {
                PermissionType = "Topic:Subscribe",
                ResourceType = "Topic",
                ResourceId = topic
            });
        }

        var expectedSubscriberGroup = new SubscriberGroupPrincipal()
        {
            Key = Guid.Parse(expectedOutput[0]),
            Name = expectedOutput[1],
            Description = expectedOutput[2],
            AvailablePermissions = expectedPermissions,
            Audience = expectedOutput[4]
        };

        expectedConsole.MarkupLine($"[green]Subscriber Group created successfully:[/]");
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.Write(new Grid()
            .AddColumns(5)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
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
            )
        );
    }
}