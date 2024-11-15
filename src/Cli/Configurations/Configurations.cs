using Microsoft.Extensions.Configuration;

namespace Qala.Cli.Configurations;

public class Configurations
{
    public static IConfiguration SetupConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        return builder.Build();
    }
}