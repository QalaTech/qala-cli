using Cli.Models;
using Cli.Services;
using Cli.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cli.Configurations;

public class Services
{
    public static void RegisterServices(IServiceCollection services, IConfiguration appSettings, Config config)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddHttpClient<ITopicService, TopicService>(client =>
        {
            var baseUrl = appSettings["PublishingApi:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("PublishingApi:BaseUrl is not configured.");
            }
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("x-auth-token", config.ApiKey);
            client.DefaultRequestHeaders.Add("x-environment-id", config.EnvironmentId);
        });

        services.AddHttpClient<ISubscriptionService, SubscriptionService>(client =>
        {
            var baseUrl = appSettings["PublishingApi:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("PublishingApi:BaseUrl is not configured.");
            }
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("x-auth-token", config.ApiKey);
            client.DefaultRequestHeaders.Add("x-environment-id", config.EnvironmentId);
        });

        services.AddSingleton<IConfigService>(new ConfigService(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".qala-cli-config")));
    }
}