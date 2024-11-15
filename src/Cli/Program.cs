using Cli.Commands.Config;
using Cli.Commands.Create;
using Cli.Commands.Create.Subscriptions;
using Cli.Commands.Create.Topics;
using Cli.Commands.Get;
using Cli.Commands.Get.EventTypes;
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
    config.AddCommand<ConfigCommand>("config")
        .WithDescription("this command creates a new config file")
        .WithExample(["config -k 'API_KEY' -e 'ENVIRONMENT_ID'"]);
    config.AddBranch<GetArgument>("get", get =>
    {
        get.AddCommand<EventTypesCommand>("eventTypes")
            .WithDescription("this command gets all or one specific event type")
            .WithExample(["get eventTypes"])
            .WithExample(["get eventTypes 'event_type_id'"]);
    });
    config.AddBranch<CreateArgument>("create", create =>
    {
        create.AddCommand<TopicsCommand>("topic")
            .WithDescription("this comand creates a new topic")
            .WithExample(["create topic topic_name -d 'This is a description for the topic' -e 'event1, event2'"]);
        create.AddCommand<SubscriptionsCommand>("subscription")
            .WithDescription("this command creates a new subscription")
            .WithExample(["create subscription subscription_name -t 'topic_name' -d 'This is a description for the subscription' -e 'event1', 'event2' -w 'https://webhook.url' -r 3"]);
    });
});

await app.RunAsync(args);