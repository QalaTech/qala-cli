using System.Reflection;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Models;
using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Integration.Tests.Fixtures;

public class QalaCliBaseFixture : IDisposable
{
    public readonly List<Models.Environment> AvailableEnvironments = new()
    {
        new() { Id = Guid.NewGuid(), Name = "TestEnv", Region = "us-east-1", EnvironmentType = "dev" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv2", Region = "us-east-2", EnvironmentType = "prod" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv3", Region = "us-east-3", EnvironmentType = "prod" }
    };

    public readonly string ApiKey = Guid.NewGuid().ToString();

    public Mock<IOrganizationService> OrganizationServiceMock = new();

    public required IMediator Mediator { get; init; }

    public QalaCliBaseFixture()
    {
        InitializeOrganizationService();

        var services = new ServiceCollection();
        InitializeServices(services);

        var serviceProvider = services.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    private void InitializeOrganizationService()
    {
        OrganizationServiceMock.Setup(
            o => o.GetOrganizationAsync())
                    .ReturnsAsync(new Organization 
                    { 
                        Name = "TestOrg", 
                        SubDomain = "testorg", 
                        Environments = AvailableEnvironments 
                    });
    }

    private void InitializeServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient<IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>, SetEnvironmentHandler>();
        services.AddSingleton<IOrganizationService>(OrganizationServiceMock.Object);
        services.AddSingleton<IEnvironmentService, EnvironmentService>();

        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddSingleton<IConfigService, ConfigService>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
