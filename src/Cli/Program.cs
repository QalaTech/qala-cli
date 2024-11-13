using Cli.Commands.Config;
using Cli.Commands.Subscriptions;
using Cli.Commands.Topics;
using Cli.DependencyInjection;
using Cli.Services;
using Cli.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration appSettings = builder.Build();

var configService = new ConfigService(Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".qala-cli-config"));
var config = configService.GetAsync().Result;

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

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("qala");
    config.AddCommand<CreateConfigCommand>("createConfig")
        .WithDescription("this command creates a new config file")
        .WithExample(["createConfig -k 'API_KEY' -e 'ENVIRONMENT_ID'"]);
    config.AddCommand<CreateTopicCommand>("createTopic")
        .WithDescription("this comand creates a new topic")
        .WithExample(["createTopic topic_name -d 'This is a description for the topic' -e 'event1' -e 'event2'"]);
    config.AddCommand<CreateSubscriptionCommand>("createSubscription")
        .WithDescription("this command creates a new subscription")
        .WithExample(["createSubscription subscription_name -t 'topic_name' -d 'This is a description for the subscription' -e 'event1' -e 'event2' -w 'https://webhook.url' -r 3"]);
});

await app.RunAsync(args);