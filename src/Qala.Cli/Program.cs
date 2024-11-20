using Qala.Cli.Commands.Config;
using Qala.Cli.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Qala.Cli.Commands.Login;

var services = new ServiceCollection();

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
});

await app.RunAsync(args);