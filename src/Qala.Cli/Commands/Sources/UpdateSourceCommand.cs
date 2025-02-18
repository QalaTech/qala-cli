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
                var configuration = BuildConfiguration(settings);
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

        if (string.IsNullOrWhiteSpace(argument.NewName))
        {
            return ValidationResult.Error("New source name is required.");
        }

        if (string.IsNullOrWhiteSpace(argument.Description))
        {
            return ValidationResult.Error("Source description is required.");
        }

        if (argument.Methods.Count == 0)
        {
            return ValidationResult.Error("At least one http method is required.");
        }

        return ValidationResult.Success();
    }

    private static SourceConfiguration BuildConfiguration(UpdateSourceArgument argument)
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
                    Password = argument.Password
                };
                break;
            case AuthSchemeType.ApiKey:
                configuration.AuthenticationScheme = new ApiKeyAuthenticationScheme
                {
                    Type = AuthSchemeType.ApiKey,
                    ApiKeyName = argument.ApiKeyName,
                    ApiKeyValue = argument.ApiKeyValue
                };
                break;
            case AuthSchemeType.JWT:
                if (!Enum.TryParse<Algorithm>(argument.Algorithm, out var algorithm))
                {
                    if (algorithm == Algorithm.RSA)
                    {
                        configuration.AuthenticationScheme = new JwtAuthenticationScheme
                        {
                            Type = AuthSchemeType.JWT,
                            Issuer = argument.Issuer,
                            Audience = argument.Audience,
                            Algorithm = Algorithm.RSA,
                            PublicKey = argument.PublicKey
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
        }

        return configuration;
    }
}