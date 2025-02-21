using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class UpdateSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string NewName, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("sources update TestSource --name TestSourceUpdatedNoAuthNoIp --description TestSourceDescriptionUpdated --methods PUT,DELETE", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sr update TestSource7 -n TestSourceUpdatedNoAuthNoIp -d TestSourceDescriptionUpdated -m PUT,DELETE", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sources update TestSource2 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sr update TestSource8 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sources update TestSource3 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType Basic --username testUsername --password testPassword", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sr update TestSource9 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a Basic --username testUsername --password testPassword", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sources update TestSource4 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType ApiKey --apiKeyName testApiKeyName --apiKeyValue testApiKeyValue", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sr update TestSource10 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a ApiKey --apiKeyName testApiKeyName --apiKeyValue testApiKeyValue", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sources update TestSource5 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType JWT --issuer testIssuer --audience testAudience --algorithm HSA --secret testSecret", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sr update TestSource11 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a JWT --issuer testIssuer --audience testAudience --algorithm HSA --secret testSecret", true, null, new string[] { "Source updated successfully." })]
    [InlineData("sources update --name TestSourceUpdatedNoAuthNoIp --description TestSourceDescriptionUpdated --methods PUT,DELETE", false, new string[] { "Source name is required." }, new string[] { "Source name is required" })]
    [InlineData("sr update -n TestSourceUpdatedNoAuthNoIp -d TestSourceDescriptionUpdated -m PUT,DELETE", false, new string[] { "Source name is required." }, new string[] { "Source name is required" })]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType Basic --password testPassword", false, new string[] { "Username is required when authentication type is basic." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a Basic --password testPassword", false, new string[] { "Username is required when authentication type is basic." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType Basic --username testUsername", false, new string[] { "Password is required when authentication type is basic." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a Basic --username testUsername", false, new string[] { "Password is required when authentication type is basic." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType ApiKey --apiKeyValue testApiKeyValue", false, new string[] { "Api key name is required when authentication type is api key." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a ApiKey --apiKeyValue testApiKeyValue", false, new string[] { "Api key name is required when authentication type is api key." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType ApiKey --apiKeyName testApiKeyName", false, new string[] { "Api key value is required when authentication type is api key." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a ApiKey --apiKeyName testApiKeyName", false, new string[] { "Api key value is required when authentication type is api key." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType JWT --audience testAudience --algorithm HSA --secret testSecret", false, new string[] { "Issuer is required when authentication type is jwt." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a JWT --audience testAudience --algorithm HSA --secret testSecret", false, new string[] { "Issuer is required when authentication type is jwt." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType JWT --issuer testIssuer --algorithm HSA --secret testSecret", false, new string[] { "Audience is required when authentication type is jwt." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a JWT --issuer testIssuer --algorithm HSA --secret testSecret", false, new string[] { "Audience is required when authentication type is jwt." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType JWT --issuer testIssuer --audience testAudience --secret testSecret", false, new string[] { "Algorithm is required when authentication type is jwt." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a JWT --issuer testIssuer --audience testAudience --secret testSecret", false, new string[] { "Algorithm is required when authentication type is jwt." }, null)]
    [InlineData("sources update TestSource6 --name TestSourceUpdatedNoAuth --description TestSourceDescriptionUpdated --methods PUT,DELETE --ip-whitelisting 100.0.0.1,101.0.0.1/24 --authenticationType JWT --issuer testIssuer --audience testAudience --algorithm HSA", false, new string[] { "Secret is required when authentication type is jwt and algorithm is HSA." }, null)]
    [InlineData("sr update TestSource6 -n TestSourceUpdatedNoAuth -d TestSourceDescriptionUpdated -m PUT,DELETE -i 100.0.0.1,101.0.0.1/24 -a JWT --issuer testIssuer --audience testAudience --algorithm HSA", false, new string[] { "Secret is required when authentication type is jwt and algorithm is HSA." }, null)]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new UpdateSourceCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "update", null);
        var (Name, NewName, Description, Methods, IpWhitelisting, AuthenticationType, Username, Password, ApiKeyName, ApiKeyValue, Issuer, Audience, Algorithm, PublicKey, Secret) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedOutput != null)
        {
            if (expectedSuccess)
            {
                ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
            }
            else
            {
                ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
            }
        }

        var inputArguments = new UpdateSourceArgument()
        {
            Name = Name,
            NewName = NewName,
            Description = Description,
            Methods = Methods,
            IpWhitelisting = IpWhitelisting,
            AuthenticationType = AuthenticationType,
            Username = Username,
            Password = Password,
            ApiKeyName = ApiKeyName,
            ApiKeyValue = ApiKeyValue,
            Issuer = Issuer,
            Audience = Audience,
            Algorithm = Algorithm,
            PublicKey = PublicKey,
            Secret = Secret
        };

        // Act
        var resultValidation = command.Validate(context, inputArguments);
        var result = -1;
        if (expectedOutput != null)
        {
            result = await command.ExecuteAsync(context, inputArguments);
        }

        // Assert
        TestsUtils.AssertValidationOutput(expectedValidationResult, resultValidation);
        TestsUtils.AssertConsoleOutput(result, expectedSuccess, expectedOutput, console, expectedConsole);
    }

    public (string Name, string NewName, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret) ExtractArgumentsValues(List<string> arguments)
    {
        var nameIndex = arguments.IndexOf("update");
        var name = string.Empty;
        if (nameIndex != -1 && nameIndex + 1 < arguments.Count)
        {
            if (!arguments[nameIndex + 1].StartsWith("-"))
            {
                name = arguments[nameIndex + 1];
            }
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

        var methodsIndex = arguments.IndexOf("-m") != -1 ? arguments.IndexOf("-m") : arguments.IndexOf("--methods");
        var methods = new List<string>();
        if (methodsIndex != -1 && methodsIndex + 1 < arguments.Count)
        {
            methods = arguments[methodsIndex + 1].Split(',').ToList();
        }

        var ipWhitelistingIndex = arguments.IndexOf("-i") != -1 ? arguments.IndexOf("-i") : arguments.IndexOf("--ip-whitelisting");
        var ipWhitelisting = new List<string>();
        if (ipWhitelistingIndex != -1 && ipWhitelistingIndex + 1 < arguments.Count)
        {
            ipWhitelisting = arguments[ipWhitelistingIndex + 1].Split(',').ToList();
        }

        var authenticationTypeIndex = arguments.IndexOf("-a") != -1 ? arguments.IndexOf("-a") : arguments.IndexOf("--authenticationType");
        var authenticationType = string.Empty;
        if (authenticationTypeIndex != -1 && authenticationTypeIndex + 1 < arguments.Count)
        {
            authenticationType = arguments[authenticationTypeIndex + 1];
        }

        var usernameIndex = arguments.IndexOf("--username");
        var username = string.Empty;
        if (usernameIndex != -1 && usernameIndex + 1 < arguments.Count)
        {
            username = arguments[usernameIndex + 1];
        }

        var passwordIndex = arguments.IndexOf("--password");
        var password = string.Empty;
        if (passwordIndex != -1 && passwordIndex + 1 < arguments.Count)
        {
            password = arguments[passwordIndex + 1];
        }

        var apiKeyNameIndex = arguments.IndexOf("--apiKeyName");
        var apiKeyName = string.Empty;
        if (apiKeyNameIndex != -1 && apiKeyNameIndex + 1 < arguments.Count)
        {
            apiKeyName = arguments[apiKeyNameIndex + 1];
        }

        var apiKeyValueIndex = arguments.IndexOf("--apiKeyValue");
        var apiKeyValue = string.Empty;
        if (apiKeyValueIndex != -1 && apiKeyValueIndex + 1 < arguments.Count)
        {
            apiKeyValue = arguments[apiKeyValueIndex + 1];
        }

        var issuerIndex = arguments.IndexOf("--issuer");
        var issuer = string.Empty;
        if (issuerIndex != -1 && issuerIndex + 1 < arguments.Count)
        {
            issuer = arguments[issuerIndex + 1];
        }

        var audienceIndex = arguments.IndexOf("--audience");
        var audience = string.Empty;
        if (audienceIndex != -1 && audienceIndex + 1 < arguments.Count)
        {
            audience = arguments[audienceIndex + 1];
        }

        var algorithmIndex = arguments.IndexOf("--algorithm");
        var algorithm = string.Empty;
        if (algorithmIndex != -1 && algorithmIndex + 1 < arguments.Count)
        {
            algorithm = arguments[algorithmIndex + 1];
        }

        var publicKeyIndex = arguments.IndexOf("--publicKey");
        var publicKey = string.Empty;
        if (publicKeyIndex != -1 && publicKeyIndex + 1 < arguments.Count)
        {
            publicKey = arguments[publicKeyIndex + 1];
        }

        var secretIndex = arguments.IndexOf("--secret");
        var secret = string.Empty;
        if (secretIndex != -1 && secretIndex + 1 < arguments.Count)
        {
            secret = arguments[secretIndex + 1];
        }

        return (name, newName, description, methods, ipWhitelisting, authenticationType, username, password, apiKeyName, apiKeyValue, issuer, audience, algorithm, publicKey, secret);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine("[red bold]Error during Source update:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[green bold]Source updated successfully.[/]");
    }
}