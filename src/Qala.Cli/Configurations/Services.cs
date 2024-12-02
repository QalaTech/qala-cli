using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Qala.Cli.Gateway;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Data.Repository;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Utils;
using Qala.Cli.Data.Gateway;
using Microsoft.Extensions.Configuration;

namespace Qala.Cli.Configurations;

public class Services
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IEventTypeService, EventTypeService>();
        services.AddSingleton<ITopicService, TopicService>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
    }

    public static void RegisterDataServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILocalEnvironments, LocalEnvironments>();
        services.AddHttpClient<IOrganizationGateway, OrganizationGateway>(client => BuildHttpClient(client, configuration));
        services.AddHttpClient<IEnvironmentGateway, EnvironmentGateway>(client => BuildHttpClient(client, configuration));
        services.AddHttpClient<IEventTypeGateway, EventTypeGateway>(client => BuildHttpClient(client, configuration));
        services.AddHttpClient<ITopicGateway, TopicGateway>(client => BuildHttpClient(client, configuration));
        services.AddHttpClient<ISubscriptionGateway, SubscriptionGateway>(client => BuildHttpClient(client, configuration));
    }

    private static void BuildHttpClient(HttpClient client, IConfiguration configuration)
    {
        var baseUrl = configuration["Management-API:URL"] ?? "https://management-api.qalatech.io/v1/";

        client.BaseAddress = new Uri(baseUrl);

        var key = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(key))
        {
            var token = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        else
        {
            client.DefaultRequestHeaders.Add("x-auth-token", key);
        }

        var environmentId = System.Environment.GetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User);
        if (!string.IsNullOrEmpty(environmentId))
        {
            client.DefaultRequestHeaders.Add("x-environment-id", environmentId);
        }
    }
}