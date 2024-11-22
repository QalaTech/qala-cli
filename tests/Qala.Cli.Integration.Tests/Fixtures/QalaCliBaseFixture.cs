using System.Reflection;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Utils;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Integration.Tests.Fixtures;

public class QalaCliBaseFixture : IDisposable
{
    public readonly List<Data.Models.Environment> AvailableEnvironments = new()
    {
        new() { Id = Guid.NewGuid(), Name = "TestEnv", Region = "us-east-1", EnvironmentType = "dev" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv2", Region = "us-east-2", EnvironmentType = "prod" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv3", Region = "us-east-3", EnvironmentType = "prod" }
    };

    public readonly string ApiKey = Guid.NewGuid().ToString();

    public Mock<IOrganizationService> OrganizationServiceMock = new();
    public Mock<ILocalEnvironments> LocalEnvironmentsMock = new();

    public required IMediator Mediator { get; init; }

    public QalaCliBaseFixture()
    {
        InitializeOrganizationService();
        InitializaLocalEnvironments();

        var services = new ServiceCollection();
        InitializeDataServices(services);
        InitializeServices(services);

        var serviceProvider = services.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
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

    private void InitializaLocalEnvironments()
    {
        LocalEnvironmentsMock.Setup(
            l => l.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]))
                    .Returns(ApiKey);

        LocalEnvironmentsMock.Setup(
            l => l.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]))
                    .Returns(AvailableEnvironments.First().Id.ToString());
    }

    private void InitializeServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient<IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>, SetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<GetEnvironmentRequest, Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>, GetEnvironmentHandler>();

        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddTransient<IConfigService, ConfigService>();
    }

    private void InitializeDataServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalEnvironments>(LocalEnvironmentsMock.Object);
        services.AddSingleton<IOrganizationService>(OrganizationServiceMock.Object);
    }
}
