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
        .WithAlias("c");
    config.AddBranch<GetArgument>("get", get =>
    {
        get.SetDescription("this command gets all or one specific resource");
        get.AddCommand<EventTypesCommand>("eventTypes")
            .WithDescription("this command gets all or one specific event type")
            .WithAlias("e");
    }).WithAlias("g");
    config.AddBranch<CreateArgument>("create", create =>
    {
        create.SetDescription("this command creates a new resource");
        create.AddCommand<TopicsCommand>("topics")
            .WithDescription("this comand creates a new topic")
            .WithAlias("t");
        create.AddCommand<SubscriptionsCommand>("subscriptions")
            .WithDescription("this command creates a new subscription")
            .WithAlias("s");
    });
});

await app.RunAsync(args);