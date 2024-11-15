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
        var baseUrl = appSettings["PublishingApi:BaseUrl"];

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddHttpClient<ITopicService, TopicService>(client => 
            BuildHttpClient(config, client, baseUrl));

        services.AddHttpClient<ISubscriptionService, SubscriptionService>(client =>
            BuildHttpClient(config, client, baseUrl));

        services.AddHttpClient<IEventTypeService, EventTypeService>(client =>
            BuildHttpClient(config, client, baseUrl));

        services.AddSingleton<IConfigService>(new ConfigService(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".qala-cli-config")));
    }

    private static void BuildHttpClient(Config config, HttpClient client, string? baseUrl)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("PublishingApi:BaseUrl is not configured.");
        }

        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("x-auth-token", config.ApiKey);
        client.DefaultRequestHeaders.Add("x-environment-id", config.EnvironmentId);
    }
}