using System.Reflection;
using System.Xml.Serialization;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Qala.Cli.Commands.Config;
using Qala.Cli.Commands.Environment;
using Qala.Cli.Gateway.Interfaces;
using Qala.Cli.Models;
using Qala.Cli.Services;
using Qala.Cli.Services.Interfaces;
using Qala.Cli.Utils;

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

    private string CurrentApiKey = string.Empty;
    private string CurrentEnvironmentId = string.Empty;
    private string CurrentAuthToken = string.Empty;

    public QalaCliBaseFixture()
    {
        SaveCurrentEnvironmentVariables();

        InitializeOrganizationService();

        var services = new ServiceCollection();
        InitializeServices(services);

        var serviceProvider = services.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    public void SetEnvironmet(Guid environmentId) => System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], environmentId.ToString(), EnvironmentVariableTarget.User);

    public void Dispose()
    {
        System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], CurrentEnvironmentId, EnvironmentVariableTarget.User);
        System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], CurrentApiKey, EnvironmentVariableTarget.User);
        System.Environment.SetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_AUTH_TOKEN], CurrentAuthToken, EnvironmentVariableTarget.User);
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

    private void InitializeServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient<IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>, SetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<GetEnvironmentRequest, Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>, GetEnvironmentHandler>();
        services.AddSingleton<IOrganizationService>(OrganizationServiceMock.Object);
        services.AddSingleton<IEnvironmentService, EnvironmentService>();

        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddSingleton<IConfigService, ConfigService>();
    }

    private void SaveCurrentEnvironmentVariables()
    {
        CurrentApiKey = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_API_KEY], EnvironmentVariableTarget.User) ?? string.Empty;
        CurrentEnvironmentId = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_ENVIRONMENT_ID], EnvironmentVariableTarget.User)?? string.Empty;
        CurrentAuthToken = System.Environment.GetEnvironmentVariable(Constants.EnvironmentVariable[EnvironmentVariableType.QALA_AUTH_TOKEN], EnvironmentVariableTarget.User)?? string.Empty;
    }
}
