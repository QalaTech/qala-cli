using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class CreateSourceCommand(IMediator mediator, IAnsiConsole console) : AsyncCommand<CreateSourceArgument>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateSourceArgument settings)
    {
        return await console.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("Processing request...", async ctx =>
            {
                var configuration = BuildConfiguration(settings);
                return await mediator.Send(new CreateSourceRequest(settings.Name, settings.Description, configuration))
                    .ToAsync()
                    .Match(
                        success =>
                        {
                            BaseCommands.DisplaySuccessMessage("Source", BaseCommands.CommandAction.Create, console);
                            console.Write(new Grid()
                                .AddColumns(4)
                                .AddRow(
                                    new Text("Id", new Style(decoration: Decoration.Bold)),
                                    new Text("Name", new Style(decoration: Decoration.Bold)),
                                    new Text("Description", new Style(decoration: Decoration.Bold)),
                                    new Text("Source Type", new Style(decoration: Decoration.Bold))
                                )
                                .AddRow(
                                    new Text(success.Source.SourceId.ToString()),
                                    new Text(success.Source.Name),
                                    new Text(success.Source.Description),
                                    new Text(success.Source.SourceType.ToString())
                                )
                            );

                            return 0;
                        },
                        error =>
                        {
                            BaseCommands.DisplayErrorMessage("Source", BaseCommands.CommandAction.Create, error.Message, console);

                            return -1;
                        }
                    );
            });
    }

    public override ValidationResult Validate(CommandContext context, CreateSourceArgument argument)
    {
        if (string.IsNullOrWhiteSpace(argument.Name))
        {
            return ValidationResult.Error("Source name is required.");
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

    private static SourceConfiguration BuildConfiguration(CreateSourceArgument argument)
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