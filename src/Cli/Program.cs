using Cli.Commands.Config;
using Cli.Commands.Subscriptions;
using Cli.Commands.Topics;
using Cli.Configurations;
using Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var services = new ServiceCollection();
var appSettings = Configurations.SetupConfiguration();

var configService = new ConfigService(Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".qala-cli-config"));
var config = configService.GetAsync().Result;

Services.RegisterServices(services, appSettings, config);

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
        .WithExample(["createTopic topic_name -d 'This is a description for the topic' -e 'event1, event2'"]);
    config.AddCommand<CreateSubscriptionCommand>("createSubscription")
        .WithDescription("this command creates a new subscription")
        .WithExample(["createSubscription subscription_name -t 'topic_name' -d 'This is a description for the subscription' -e 'event1', 'event2' -w 'https://webhook.url' -r 3"]);
});

await app.RunAsync(args);