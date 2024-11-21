using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Qala.Cli.Utils;
using Qala.Cli.Gateway.Interfaces;
using Qala.Cli.Gateway;

namespace Qala.Cli.Configurations;

public class Services
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddHttpClient<IOrganizationService, OrganizationService>(BuildHttpClient);
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
    }

    private static void BuildHttpClient(HttpClient client)
    {
        var baseUrl = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_MANAGEMENT_API_URL], EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("ManagementApi:BaseUrl is not configured.");
        }

        client.BaseAddress = new Uri(baseUrl);

        var key = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(key))
        {
            var token = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_AUTH_TOKEN], EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No authentication was provided.");
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        else
        {
            client.DefaultRequestHeaders.Add("x-auth-token", key);
        }
    }
}