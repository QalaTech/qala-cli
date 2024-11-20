using Qala.Cli.Models;
using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Qala.Cli.Configurations;

public class Services
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IConfigService, ConfigService>();
    }
}