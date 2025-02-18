using Qala.Cli.Commands.Config;
using Qala.Cli.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Login;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Commands.EventTypes;
using Qala.Cli.Commands.Topics;
using Qala.Cli.Commands.Subscriptions;
using Spectre.Console;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Commands.GenerateMarkdown;

var services = new ServiceCollection();
var configuration = Configurations.SetupConfiguration();

services.AddSingleton<IAnsiConsole>(AnsiConsole.Console);
Services.RegisterDataServices(services, configuration);
Services.RegisterServices(services, configuration);

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("qala");

    config.AddCommand<GenerateMarkdownCommand>("markdown")
        .WithDescription("Generates Markdown documentation from the XML documentation file.")
        .IsHidden();

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
            .WithExample("environment create -n <NAME> -r <REGION> -t <TYPE> --disableSchemaValidation");
        env.AddCommand<GetEnvironmentCommand>("current")
            .WithDescription("this command retrieves the current environment for the Qala CLI.")
            .WithExample("environment current");
        env.AddCommand<SetEnvironmentCommand>("set")
            .WithDescription("this command sets the current environment for the Qala CLI.")
            .WithExample("environment set -e <ENVIRONMENT_ID>");
        env.AddCommand<UpdateEnvironmentCommand>("update")
            .WithDescription("this command updates the current environment for the Qala CLI.")
            .WithExample("environment update --disableSchemaValidation");
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
            .WithExample("events inspect <EVENT_TYPE_NAME>")
            .WithAlias("i");
    })
    .WithAlias("ev");

    config.AddBranch("topics", t =>
   {
       t.AddCommand<ListTopicsCommand>("list")
           .WithDescription("this command lists all the topics available in your environment.")
           .WithExample("topics ls")
           .WithAlias("ls");
       t.AddCommand<GetTopicCommand>("inspect")
           .WithDescription("this command retrieves the topic with the specified NAME.")
           .WithExample("topics inspect <NAME>")
           .WithAlias("i");
       t.AddCommand<CreateTopicCommand>("create")
           .WithDescription("this command creates a new topic for the Qala CLI.")
           .WithExample("topics create -n <NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_NAMES>");
       t.AddCommand<UpdateTopicCommand>("update")
           .WithDescription("this command updates an existing topic for the Qala CLI.")
           .WithExample("topics update <NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_NAMES>");
   })
   .WithAlias("tp");

    config.AddBranch("sources", s =>
    {
        s.AddCommand<ListSourcesCommand>("list")
           .WithDescription("this command lists all the sources available in your environment.")
           .WithExample("qala sources ls")
           .WithAlias("ls");
        s.AddCommand<GetSourceCommand>("inspect")
           .WithDescription("this command retrieves the source with the specified NAME.")
           .WithExample("qala sources inspect <NAME>")
           .WithAlias("i");
        s.AddCommand<CreateSourceCommand>("create")
           .WithDescription("this command creates a new source for the Qala CLI.")
           .WithExample("qala sources create -n <NAME> -d <DESCRIPTION> -m <METHODS_COMMA_SEPERATED> -i <IP_WHITELISTING_COMMA_SEPERATED> -a <AUTHENTICATION_TYPE>");
        s.AddCommand<UpdateSourceCommand>("update")
           .WithDescription("this command updates an existing source for the Qala CLI.")
           .WithExample("qala sources update <NAME> -d <DESCRIPTION> -m <METHODS_COMMA_SEPERATED> -i <IP_WHITELISTING_COMMA_SEPERATED> -a <AUTHENTICATION_TYPE>");
        s.AddCommand<DeleteSourceCommand>("delete")
           .WithDescription("this command deletes the source with the specified NAME.")
           .WithExample("qala sources delete -n <NAME>");
    })
     .WithAlias("sr");

    config.AddBranch("subscriptions", s =>
    {
        s.AddCommand<ListSubscriptionsCommand>("list")
            .WithDescription("this command lists all the subscriptions available in your environment for a specific topic or source.")
            .WithExample("qala subscriptions ls --topic <TOPIC_NAME>")
            .WithExample("qala subscriptions ls --source <SOURCE_NAME>")
            .WithAlias("ls");
        s.AddCommand<GetSubscriptionCommand>("inspect")
            .WithDescription("this command retrieves the subscription with the specified ID.")
            .WithExample("qala subscriptions i --topic <TOPIC_NAME> -s <SUBSCRIPTION_NAME>")
            .WithExample("qala subscriptions i --source <SOURCE_NAME> -s <SUBSCRIPTION_NAME>")
            .WithAlias("i");
        s.AddCommand<CreateSubscriptionCommand>("create")
            .WithDescription("this command creates a new subscription for the Qala CLI.")
            .WithExample("qala sub create -n <SUBSCRIPTION_NAME> --topic <TOPIC_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_NAMES> -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>");
        s.AddCommand<UpdateSubscriptionCommand>("update")
            .WithDescription("this command updates an existing subscription for the Qala CLI.")
            .WithExample("qala sub update <SUBSCRIPTION_NAME> --topic <TOPIC_NAME> -n <NEW_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_NAMES> -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>");
        s.AddCommand<DeleteSubscriptionCommand>("delete")
            .WithDescription("this command deletes the subscription with the specified ID.")
            .WithExample("qala sub delete --topic <TOPIC_NAME> -s <SUBSCRIPTION_NAME>");
        s.AddBranch("secret", ws =>
        {
            ws.AddCommand<GetWebhookSecretCommand>("inspect")
                .WithDescription("this command retrieves the webhook secret for the subscription with the specified ID.")
                .WithExample("qala sub secret i --topic <TOPIC_NAME> -s <SUBSCRIPTION_NAME>")
                .WithAlias("i");
            ws.AddCommand<RotateWebhookSecretCommand>("rotate")
                .WithDescription("this command rotates the webhook secret for the subscription with the specified ID.")
                .WithExample("qala sub secret rotate --topic <TOPIC_NAME> -s <SUBSCRIPTION_NAME>")
                .WithAlias("r");
        });
    })
    .WithAlias("sub");
});

await app.RunAsync(args);