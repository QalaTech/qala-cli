using Qala.Cli.Commands.Config;
using Qala.Cli.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Login;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Utils;
using Qala.Cli.Commands.EventTypes;
using LanguageExt;

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
        et.AddCommand<GetEventTypeCommand>("get")
            .WithDescription("this command retrieves the event type with the specified ID.")
            .WithExample("events inspect <EVENT_TYPE_ID>")
            .WithAlias("i");
    }) 
    .WithAlias("ev");   
});

await app.RunAsync(args);