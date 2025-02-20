using Moq;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Data.Models;
using Qala.Cli.Integration.Tests.Fixtures;
using Qala.Cli.Integration.Tests.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Qala.Cli.Integration.Tests.Commands;

public class CreateSourceCommandShould(QalaCliBaseFixture fixture) : IClassFixture<QalaCliBaseFixture>, ITestExecution<(string Name, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret)>
{
    private readonly IRemainingArguments _remainingArguments = new Mock<IRemainingArguments>().Object;

    [Theory]
    [InlineData("source create -n SourceNoAuthNoIp -d newly-source-description -m GET,POST", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "SourceNoAuthNoIp", "newly-source-description", "GET,POST", "", "", "", "", "", "", "", "", "", "", "" })]
    [InlineData("source create -n SourceNoAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "SourceNoAuth", "newly-source-description", "GET,POST", "127.0.0.1,110.0.0.1/24", "", "", "", "", "", "", "", "", "", "" })]
    [InlineData("source create -n SourceBasicAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a Basic --username testuser --password testpassword", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "SourceBasicAuth", "newly-source-description", "GET,POST", "127.0.0.1,110.0.0.1/24", "Basic", "testuser", "testpassword", "", "", "", "", "", "", "" })]
    [InlineData("source create -n SourceApiKeyAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a ApiKey --apiKeyName testkeyname --apiKeyValue testkeyvalue", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "SourceApiKeyAuth", "newly-source-description", "GET,POST", "127.0.0.1,110.0.0.1/24", "ApiKey", "", "", "testkeyname", "testkeyvalue", "", "", "", "", "" })]
    [InlineData("source create -n SourceJWTAuthHsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --issuer issuerValue --audience audienceValue --algorithm HSA --secret secretValue", true, null, new string[] { "60ef03bb-f5a7-4c81-addf-38e2b360bff5", "SourceJWTAuthHsa", "newly-source-description", "GET,POST", "127.0.0.1,110.0.0.1/24", "JWT", "", "", "", "", "issuerValue", "audienceValue", "HSA", "", "secretValue" })]
    [InlineData("source create -d newly-source-description -m GET,POST", false, new string[] { "Source name is required." }, new string[] { "Name is required" })]
    [InlineData("source create -n SourceNoAuth -m GET,POST", false, new string[] { "Source description is required." }, new string[] { "Description is required" })]
    [InlineData("source create -n SourceNoAuth -d newly-source-description", false, new string[] { "At least one http method is required." }, null)]
    [InlineData("source create -n SourceBasicAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a Basic --password testpassword", false, new string[] { "Username is required when authentication type is basic." }, null)]
    [InlineData("source create -n SourceBasicAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a Basic --username testusername", false, new string[] { "Password is required when authentication type is basic." }, null)]
    [InlineData("source create -n SourceApiKeyAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a ApiKey testkeyname --apiKeyValue testkeyvalue", false, new string[] { "Api key name is required when authentication type is api key." }, null)]
    [InlineData("source create -n SourceApiKeyAuth -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a ApiKey --apiKeyName testkeyname", false, new string[] { "Api key value is required when authentication type is api key." }, null)]
    [InlineData("source create -n SourceJWTAuthRsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --audience audienceValue --algorithm RSA --publicKey publicKeyFile", false, new string[] { "Issuer is required when authentication type is jwt." }, null)]
    [InlineData("source create -n SourceJWTAuthRsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --issuer issuerValue --algorithm RSA --publicKey publicKeyFile", false, new string[] { "Audience is required when authentication type is jwt." }, null)]
    [InlineData("source create -n SourceJWTAuthRsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --issuer issuerValue --audience audienceValue --publicKey publicKeyFile", false, new string[] { "Algorithm is required when authentication type is jwt." }, null)]
    [InlineData("source create -n SourceJWTAuthRsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --issuer issuerValue --audience audienceValue --algorithm RSA", false, new string[] { "Public key is required when authentication type is jwt and algorithm is RSA." }, null)]
    [InlineData("source create -n SourceJWTAuthRsa -d newly-source-description -m GET,POST -i 127.0.0.1,110.0.0.1/24 -a JWT --issuer issuerValue --audience audienceValue --algorithm HSA", false, new string[] { "Secret is required when authentication type is jwt and algorithm is HSA." }, null)]
    public async Task Execute(string input, bool expectedSuccess, string[] expectedValidationResult, string[] expectedOutput)
    {
        // Arrange
        var console = new TestConsole();
        var command = new CreateSourceCommand(fixture.Mediator, console);
        var arguments = input.Split(' ').ToList();
        var context = new CommandContext(arguments, _remainingArguments, "create", null);
        var (Name, Description, Methods, IpWhitelisting, AuthenticationType, Username, Password, ApiKeyName, ApiKeyValue, Issuer, Audience, Algorithm, PublicKey, Secret) = ExtractArgumentsValues(arguments);

        var expectedConsole = new TestConsole();
        if (expectedSuccess)
        {
            ExtractSuccessExpectedOutput(expectedOutput, expectedConsole);
        }
        else if (expectedOutput != null)
        {
            ExtractFailedExpectedOutput(expectedOutput, expectedConsole);
        }

        var inputArguments = new CreateSourceArgument()
        {
            Name = Name,
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

    public (string Name, string Description, List<string> Methods, List<string> IpWhitelisting, string AuthenticationType, string Username, string Password, string ApiKeyName, string ApiKeyValue, string Issuer, string Audience, string Algorithm, string PublicKey, string Secret) ExtractArgumentsValues(List<string> arguments)
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

        var methodsIndex = arguments.IndexOf("--methods") != -1 ? arguments.IndexOf("--methods") : arguments.IndexOf("-m");
        var methods = new List<string>();
        if (methodsIndex != -1 && methodsIndex + 1 < arguments.Count)
        {
            methods = arguments[methodsIndex + 1].Split(',').ToList();
        }

        var ipWhitelistingIndex = arguments.IndexOf("--ipWhitelisting") != -1 ? arguments.IndexOf("--ipWhitelisting") : arguments.IndexOf("-i");
        var ipWhitelisting = new List<string>();
        if (ipWhitelistingIndex != -1 && ipWhitelistingIndex + 1 < arguments.Count)
        {
            ipWhitelisting = arguments[ipWhitelistingIndex + 1].Split(',').ToList();
        }

        var authenticationTypeIndex = arguments.IndexOf("--authenticationType") != -1 ? arguments.IndexOf("--authenticationType") : arguments.IndexOf("-a");
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

        return (name, description, methods, ipWhitelisting, authenticationType, username, password, apiKeyName, apiKeyValue, issuer, audience, algorithm, publicKey, secret);
    }

    public void ExtractFailedExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.MarkupLine($"[red bold]Error during Source creation:[/]");
        expectedConsole.MarkupLine($"[red]{expectedOutput[0]}[/]");
    }

    public void ExtractSuccessExpectedOutput(string[] expectedOutput, TestConsole expectedConsole)
    {
        var expectedSource = new Source()
        {
            SourceId = Guid.Parse(expectedOutput[0]),
            Name = expectedOutput[1],
            Description = expectedOutput[2],
            SourceType = SourceType.Http,
        };

        expectedSource.Configuration = new SourceConfiguration()
        {
            AllowedHttpMethods = expectedOutput[3].Split(',').Select(m => Enum.Parse<Data.Models.HttpMethod>(m)).ToList(),
            WhitelistedIpRanges = expectedOutput[4].Split(',').ToList()
        };

        var authSchemeType = string.IsNullOrWhiteSpace(expectedOutput[5]) ? AuthSchemeType.None : Enum.Parse<AuthSchemeType>(expectedOutput[5]);
        switch (authSchemeType)
        {
            case AuthSchemeType.Basic:
                expectedSource.Configuration.AuthenticationScheme = new BasicAuthenticationScheme()
                {
                    Type = authSchemeType,
                    Username = expectedOutput[6],
                    PasswordReference = expectedOutput[7]
                };
                break;
            case AuthSchemeType.ApiKey:
                expectedSource.Configuration.AuthenticationScheme = new ApiKeyAuthenticationScheme()
                {
                    Type = authSchemeType,
                    ApiKeyName = expectedOutput[8],
                    ApiKeyValueReference = expectedOutput[9]
                };
                break;
            case AuthSchemeType.JWT:
                expectedSource.Configuration.AuthenticationScheme = new JwtAuthenticationScheme()
                {
                    Type = authSchemeType,
                    Issuer = expectedOutput[10],
                    Audience = expectedOutput[11],
                    Algorithm = Enum.Parse<Algorithm>(expectedOutput[12]),
                    PublicKey = expectedOutput[13],
                    Secret = expectedOutput[14]
                };
                break;
        }

        expectedConsole.MarkupLine($"[green]Source created successfully:[/]");
        expectedConsole.MarkupLine("Processing request...");
        expectedConsole.Write(new Grid()
            .AddColumns(4)
            .AddRow(
                new Text("Id", new Style(decoration: Decoration.Bold)),
                new Text("Name", new Style(decoration: Decoration.Bold)),
                new Text("Description", new Style(decoration: Decoration.Bold)),
                new Text("Source Type", new Style(decoration: Decoration.Bold))
            )
            .AddRow(
                new Text(expectedSource!.SourceId.ToString()),
                new Text(expectedSource.Name),
                new Text(expectedSource.Description),
                new Text(expectedSource.SourceType.ToString())
            )
        );
    }
}