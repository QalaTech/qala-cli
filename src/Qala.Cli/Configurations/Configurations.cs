using Microsoft.Extensions.Configuration;

namespace Qala.Cli.Configurations;

public class Configurations
{
    public static IConfiguration SetupConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yaml", optional: false, reloadOnChange: true);
        return builder.Build();
    }
}