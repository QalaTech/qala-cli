using Qala.Cli.Commands.Config;
using Qala.Cli.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Login;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Utils;
using Qala.Cli.Commands.EventTypes;
using LanguageExt;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Commands.Subscriptions;

Environment.SetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_MANAGEMENT_API_URL], "https://localhost:7143/v1/", EnvironmentVariableTarget.User);

var services = new ServiceCollection();

Services.RegisterDataServices(services);
Services.RegisterServices(services);

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("qala");

    config.AddCommand<LoginCommand>("login")
        .WithDescription("this commands initiates the process for the user to login.")
        .WithExample("login");

    config.AddCommand<ConfigCommand>("config")
        .WithDescription("this command creates enables the configuration of the Qala CLI (for automation purposes).")
        .WithExample("config -k <API_KEY> -e <ENV_ID>");

    config.AddBranch("environment", env =>
    {
        env.AddCommand<CreateEnvironmentCommand>("create")
            .WithDescription("this command creates a new environment for the Qala CLI.")
            .WithExample("environment create -n <NAME> -r <REGION> -t <TYPE>");
        env.AddCommand<GetEnvironmentCommand>("current")
            .WithDescription("this command retrieves the current environment for the Qala CLI.")
            .WithExample("environment current");
        env.AddCommand<SetEnvironmentCommand>("set")
            .WithDescription("this command sets the current environment for the Qala CLI.")
            .WithExample("environment set -e <ENVIRONMENT_ID>");
    })
    .WithAlias("env");

    config.AddBranch("events", et =>
    {
        et.AddCommand<ListEventTypesCommand>("list")
            .WithDescription("this command lists all the event types available in your environment.")
            .WithExample("events ls")
            .WithAlias("ls");
        et.AddCommand<GetEventTypeCommand>("inspect")
            .WithDescription("this command retrieves the event type with the specified ID.")
            .WithExample("events inspect <EVENT_TYPE_ID>")
            .WithAlias("i");
    }) 
    .WithAlias("ev");  

     config.AddBranch("topics", t =>
    {
        t.AddCommand<ListTopicsCommand>("list")
            .WithDescription("this command lists all the topics available in your environment.")
            .WithExample("topics ls")
            .WithAlias("ls");
        t.AddCommand<GetTopicCommand>("name")
            .WithDescription("this command retrieves the topic with the specified NAME.")
            .WithExample("topics inspect <NAME>")
            .WithAlias("n");
        t.AddCommand<CreateTopicCommand>("create")
            .WithDescription("this command creates a new topic for the Qala CLI.")
            .WithExample("topics create -n <NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS>");
        t.AddCommand<UpdateTopicCommand>("update")
            .WithDescription("this command updates an existing topic for the Qala CLI.")
            .WithExample("topics update <NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS>");
    })
    .WithAlias("tp");

    config.AddBranch("subscriptions", s =>
    {
        s.AddCommand<ListSubscriptionsCommand>("list")
            .WithDescription("this command lists all the subscriptions available in your environment.")
            .WithExample("qala subscriptions ls -t <TOPIC_NAME>")
            .WithAlias("ls");
        s.AddCommand<GetSubscriptionCommand>("inspect")
            .WithDescription("this command retrieves the subscription with the specified ID.")
            .WithExample("qala subscriptions i -t <TOPIC_NAME> -s <SUBSCRIPTION_ID>")
            .WithAlias("i");
        s.AddCommand<CreateSubscriptionCommand>("create")
            .WithDescription("this command creates a new subscription for the Qala CLI.")
            .WithExample("qala sub create -n <SUBSCRIPTION_NAME> -t <TOPIC_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS> -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>");
        s.AddCommand<UpdateSubscriptionCommand>("update")
            .WithDescription("this command updates an existing subscription for the Qala CLI.")
            .WithExample("qala sub update <SUBSCRIPTION_ID> -t <TOPIC_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS> -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>");
        s.AddCommand<DeleteSubscriptionCommand>("delete")
            .WithDescription("this command deletes the subscription with the specified ID.")
            .WithExample("qala sub delete -t <TOPIC_NAME> -s <SUBSCRIPTION_ID>");
    })
    .WithAlias("sub");
});

await app.RunAsync(args);