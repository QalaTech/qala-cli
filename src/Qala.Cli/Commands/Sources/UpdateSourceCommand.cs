using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class UpdateSourceCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<UpdateSourceArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, UpdateSourceArgument settings)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                var configuration = await BuildConfigurationAsync(settings);
                return await mediator.Send(new UpdateSourceRequest(settings.Name, settings.NewName, settings.Description, configuration))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Source", BaseCommands.CommandAction.Update, console);

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Source", BaseCommands.CommandAction.Update, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, UpdateSourceArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Source name is required.");
        }

        if (!string.IsNullOrWhiteSpace(argument.AuthenticationType))
        {
            if (argument.AuthenticationType.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(argument.Username))
                {
                    return ValidationResult.Error("Username is required when authentication type is basic.");
                }

                if (string.IsNullOrWhiteSpace(argument.Password))
                {
                    return ValidationResult.Error("Password is required when authentication type is basic.");
                }
            }
            else if (argument.AuthenticationType.Equals("ApiKey", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(argument.ApiKeyName))
                {
                    return ValidationResult.Error("Api key name is required when authentication type is api key.");
                }

                if (string.IsNullOrWhiteSpace(argument.ApiKeyValue))
                {
                    return ValidationResult.Error("Api key value is required when authentication type is api key.");
                }
            }
            else if (argument.AuthenticationType.Equals("JWT", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(argument.Issuer))
                {
                    return ValidationResult.Error("Issuer is required when authentication type is jwt.");
                }

                if (string.IsNullOrWhiteSpace(argument.Audience))
                {
                    return ValidationResult.Error("Audience is required when authentication type is jwt.");
                }

                if (string.IsNullOrWhiteSpace(argument.Algorithm))
                {
                    return ValidationResult.Error("Algorithm is required when authentication type is jwt.");
                }

                if (argument.Algorithm.Equals("RSA", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(argument.PublicKey))
                    {
                        return ValidationResult.Error("Public key is required when authentication type is jwt and algorithm is RSA.");
                    }
                }
                else if (argument.Algorithm.Equals("HSA", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(argument.Secret))
                    {
                        return ValidationResult.Error("Secret is required when authentication type is jwt and algorithm is HSA.");
                    }
                }
            }
        }

        return ValidationResult.Success();
    }

    private static async Task<SourceConfiguration> BuildConfigurationAsync(UpdateSourceArgument argument)
    {
        var configuration = new SourceConfiguration
        {
            AllowedHttpMethods = [.. argument.Methods.Select(method => Enum.Parse<Data.Models.HttpMethod>(method, true))]
        };

        if (argument.IpWhitelisting != null)
        {
            configuration.WhitelistedIpRanges = argument.IpWhitelisting;
        }

        if (string.IsNullOrWhiteSpace(argument.AuthenticationType))
        {
            return configuration;
        }

        var authScheme = Enum.Parse<AuthSchemeType>(argument.AuthenticationType);

        switch (authScheme)
        {
            case AuthSchemeType.Basic:
                configuration.AuthenticationScheme = new BasicAuthenticationScheme
                {
                    Type = AuthSchemeType.Basic,
                    Username = argument.Username,
                    PasswordReference = argument.Password
                };
                break;
            case AuthSchemeType.ApiKey:
                configuration.AuthenticationScheme = new ApiKeyAuthenticationScheme
                {
                    Type = AuthSchemeType.ApiKey,
                    ApiKeyName = argument.ApiKeyName,
                    ApiKeyValueReference = argument.ApiKeyValue
                };
                break;
            case AuthSchemeType.JWT:
                if (Enum.TryParse<Algorithm>(argument.Algorithm, out var algorithm))
                {
                    if (algorithm == Algorithm.RSA)
                    {
                        var publicKey = await File.ReadAllTextAsync(argument.PublicKey);
                        configuration.AuthenticationScheme = new JwtAuthenticationScheme
                        {
                            Type = AuthSchemeType.JWT,
                            Issuer = argument.Issuer,
                            Audience = argument.Audience,
                            Algorithm = Algorithm.RSA,
                            PublicKey = publicKey
                        };
                    }
                    else
                    {
                        configuration.AuthenticationScheme = new JwtAuthenticationScheme
                        {
                            Type = AuthSchemeType.JWT,
                            Issuer = argument.Issuer,
                            Audience = argument.Audience,
                            Algorithm = Algorithm.HSA,
                            Secret = argument.Secret
                        };
                    }
                }
                break;
            default:
                configuration.AuthenticationScheme = null;
                break;
        }

        return configuration;
    }
}