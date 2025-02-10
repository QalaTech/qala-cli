using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Fact]
    public async Task Execute_WithValidParameters_NoAuthentication_NoWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods });

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
    public async Task Execute_WithValidParameters_NoAuthentication_WithWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1" };
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods), "-i", string.Join(",", whitelistedIpRanges) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods, IpWhitelisting = whitelistedIpRanges });

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
    public async Task Execute_WithValidParameters_Basic_WithWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1/24", "127.0.0.1", "127.0.0.2" };
        var password = "password";
        var username = "username";
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods), "-a", "basic", "--username", username, "--password", password, "-i", string.Join(",", whitelistedIpRanges) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods, Username = username, Password = password, IpWhitelisting = whitelistedIpRanges });

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
    public async Task Execute_WithValidParameters_ApiKey_WithWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1/24", "127.0.0.1", "127.0.0.2" };
        var apiKeyName = "apiKeyName";
        var apiKeyValue = "apiKeyValue";
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods), "-a", "apikey", "--apikeyname", apiKeyName, "--apikeyvalue", apiKeyValue, "-i", string.Join(",", whitelistedIpRanges) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods, ApiKeyName = apiKeyName, ApiKeyValue = apiKeyValue, IpWhitelisting = whitelistedIpRanges });

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
    public async Task Execute_WithValidParameters_Jwt_RSA_WithWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1/24", "127.0.0.1", "127.0.0.2" };
        var issuer = "issuer";
        var audience = "audience";
        var algorithm = "RSA";
        var publicKey = "public_key";
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods), "-a", "jwt", "--issuer", issuer, "--audience", audience, "--algorithm", algorithm, "--publickey", publicKey, "-i", string.Join(",", whitelistedIpRanges) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods, Issuer = issuer, Audience = audience, Algorithm = algorithm, PublicKey = publicKey, IpWhitelisting = whitelistedIpRanges });

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
    public async Task Execute_WithValidParameters_Jwt_HSA_WithWhitelisting()
    {
        // Arrange
        var console = new TestConsole();
        var sourceName = "NewlyCreatedTestSource";
        var sourceDescription = "newly-created-source-description";
        var methods = new List<string> { "GET", "POST", "PUT", "DELETE" };
        var whitelistedIpRanges = new List<string> { "102.0.0.1/24", "127.0.0.1", "127.0.0.2" };
        var issuer = "issuer";
        var audience = "audience";
        var algorithm = "HSA";
        var secret = "secret";
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = new List<string> { "source", "create", "-n", sourceName, "-d", sourceDescription, "-m", string.Join(",", methods), "-a", "jwt", "--issuer", issuer, "--audience", audience, "--algorithm", algorithm, "--secret", secret, "-i", string.Join(",", whitelistedIpRanges) };
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var expectedOutput = new TestConsole();
        expectedOutput.Status()
                    .AutoRefresh(true)
                    .Spinner(Spinner.Known.Star2)
                    .SpinnerStyle(Style.Parse("yellow bold"))
                    .Start("Processing request...", ctx =>
                    {
                        expectedOutput.MarkupLine($"[green bold]Source created successfully:[/]");
                        expectedOutput.Write(new Grid()
                            .AddColumns(4)
                            .AddRow(
                                new Text("Id", new Style(decoration: Decoration.Bold)),
                                new Text("Name", new Style(decoration: Decoration.Bold)),
                                new Text("Description", new Style(decoration: Decoration.Bold)),
                                new Text("Source Type", new Style(decoration: Decoration.Bold))
                            )
                            .AddRow(
                                new Text("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                                new Text(sourceName),
                                new Text(sourceDescription),
                                new Text(Data.Models.SourceType.Http.ToString())
                            )
                        );
                    });

        // Act
        var result = await command.ExecuteAsync(context, new CreateSourceArgument() { Name = sourceName, Description = sourceDescription, Methods = methods, Issuer = issuer, Audience = audience, Algorithm = algorithm, Secret = secret, IpWhitelisting = whitelistedIpRanges });

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