using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Qala.Cli.Gateway;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Data.Repository;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Utils;
using Qala.Cli.Data.Gateway;

namespace Qala.Cli.Configurations;

public class Services
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IEventTypeService, EventTypeService>();
    }

    public static void RegisterDataServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalEnvironments, LocalEnvironments>();
        services.AddHttpClient<IOrganizationGateway, OrganizationGateway>(BuildHttpClient);
        services.AddHttpClient<IEnvironmentGateway, EnvironmentGateway>(BuildHttpClient);
        services.AddHttpClient<IEventTypeGateway, EventTypeGateway>(BuildHttpClient);
    }

    private static void BuildHttpClient(HttpClient client)
    {
        var baseUrl = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_MANAGEMENT_API_URL], EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("ManagementApi:BaseUrl is not configured.");
        }

        client.BaseAddress = new Uri(baseUrl);

        var key = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(key))
        {
            var token = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No authentication was provided.");
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        else
        {
            client.DefaultRequestHeaders.Add("x-auth-token", key);
            var environmentId = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(environmentId))
            {
                client.DefaultRequestHeaders.Add("x-environment-id", environmentId);
            }
        }
    }
}