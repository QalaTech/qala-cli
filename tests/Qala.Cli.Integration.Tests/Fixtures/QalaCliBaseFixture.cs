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
using Qala.Cli.Commands.EventTypes;

namespace Qala.Cli.Integration.Tests.Fixtures;

public class QalaCliBaseFixture : IDisposable
{
    public readonly List<Data.Models.Environment> AvailableEnvironments = new()
    {
        new() { Id = Guid.NewGuid(), Name = "TestEnv", Region = "us-east-1", EnvironmentType = "dev" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv2", Region = "us-east-2", EnvironmentType = "prod" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv3", Region = "us-east-3", EnvironmentType = "prod" }
    };

    public readonly List<Data.Models.EventType> AvailableEventTypes = new()
    {
        new() { Id = Guid.NewGuid(), Type = "Type 1", Description = "Test Event Description", Schema="{\"name\":\"Test\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat1", "cat2" } },
        new() { Id = Guid.NewGuid(), Type = "Type 2", Description = "Test Event Description 2", Schema="{\"name\":\"Test2\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat3", "cat4" } },
        new() { Id = Guid.NewGuid(), Type = "Type 3", Description = "Test Event Description 3", Schema="{\"name\":\"Test3\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat5", "cat6" } }
    };

    public readonly string ApiKey = Guid.NewGuid().ToString();

    public Mock<IOrganizationGateway> OrganizationServiceMock = new();
    public Mock<ILocalEnvironments> LocalEnvironmentsMock = new();
    public Mock<IEnvironmentGateway> EnvironmentGatewayMock = new();
    public Mock<IEventTypeGateway> EventTypeGatewayMock = new();

    public required IMediator Mediator { get; init; }

    public QalaCliBaseFixture()
    {
        InitializaLocalEnvironmentsMock();
        InitializeOrganizationGatewayMock();
        InitializeEnvironmentGatewayMock();
        InitializeEventTypeGatewayMock();

        var services = new ServiceCollection();
        InitializeDataServices(services);
        InitializeCommandHandlers(services);
        InitializeServices(services);

        var serviceProvider = services.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void InitializaLocalEnvironmentsMock()
    {
        LocalEnvironmentsMock.Setup(
            l => l.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]))
                    .Returns(ApiKey);

        LocalEnvironmentsMock.Setup(
            l => l.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]))
                    .Returns(AvailableEnvironments.First().Id.ToString());
    }

    private void InitializeOrganizationGatewayMock()
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

    private void InitializeEnvironmentGatewayMock()
    {
        EnvironmentGatewayMock.Setup(
            e => e.CreateEnvironmentAsync(It.IsAny<Data.Models.Environment>()))
                    .ReturnsAsync(() => {
                        var newEnvironment = new Data.Models.Environment
                        {
                            Id = Guid.NewGuid(),
                            Name = "NewlyCreatedTestEnv",
                            Region = "newly-region",
                            EnvironmentType = "newly-env-type"
                        };

                        AvailableEnvironments.Add(newEnvironment);

                        return newEnvironment;
                    });
    }

    private void InitializeEventTypeGatewayMock()
    {
        EventTypeGatewayMock.Setup(
            e => e.ListEventTypesAsync())
                    .ReturnsAsync(AvailableEventTypes);
        
        EventTypeGatewayMock.Setup(
            e => e.GetEventTypeAsync(It.IsAny<Guid>()))
                    .ReturnsAsync((Guid id) => AvailableEventTypes.FirstOrDefault(et => et.Id == id));
    }

    private static void InitializeCommandHandlers(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient<IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>, SetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<GetEnvironmentRequest, Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>, GetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<CreateEnvironmentRequest, Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>, CreateEnvironmentHandler>();
        services.AddTransient<IRequestHandler<ListEventTypesRequest, Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>, ListEventTypesHandler>();
        services.AddTransient<IRequestHandler<GetEventTypeRequest, Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>, GetEventTypeHandler>();
    }

    private static void InitializeServices(IServiceCollection services)
    {
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IEventTypeService, EventTypeService>();
    }

    private void InitializeDataServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalEnvironments>(LocalEnvironmentsMock.Object);
        services.AddSingleton<IOrganizationGateway>(OrganizationServiceMock.Object);
        services.AddSingleton<IEnvironmentGateway>(EnvironmentGatewayMock.Object);
        services.AddSingleton<IEventTypeGateway>(EventTypeGatewayMock.Object);
    }
}
