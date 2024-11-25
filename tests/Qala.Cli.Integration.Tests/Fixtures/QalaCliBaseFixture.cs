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
using Qala.Cli.Commands.Topics;
using Qala.Cli.Commands.Subscriptions;

namespace Qala.Cli.Integration.Tests.Fixtures;

public class QalaCliBaseFixture : IDisposable
{
    public readonly List<Data.Models.Environment> AvailableEnvironments = new()
    {
        new() { Id = Guid.NewGuid(), Name = "TestEnv", Region = "us-east-1", EnvironmentType = "dev" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv2", Region = "us-east-2", EnvironmentType = "prod" },
        new() { Id = Guid.NewGuid(), Name = "TestEnv3", Region = "us-east-3", EnvironmentType = "prod" }
    };

    public readonly List<EventType> AvailableEventTypes = new()
    {
        new() { Id = Guid.NewGuid(), Type = "Type 1", Description = "Test Event Description", Schema="{\"name\":\"Test\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat1", "cat2" } },
        new() { Id = Guid.NewGuid(), Type = "Type 2", Description = "Test Event Description 2", Schema="{\"name\":\"Test2\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat3", "cat4" } },
        new() { Id = Guid.NewGuid(), Type = "Type 3", Description = "Test Event Description 3", Schema="{\"name\":\"Test3\"}", ContentType="application/json", Encoding="utf-8", Categories = new List<string> { "cat5", "cat6" } }
    };

    public List<Topic> AvailableTopics = new()
    {
        new() { Id = Guid.NewGuid(), Name = "TestTopic", Description = "Test Topic Description", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic2", Description = "Test Topic Description 2", ProvisioningState = "Provisioned" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic3", Description = "Test Topic Description 3", ProvisioningState = "Provisioned" }
    };

    public List<Subscription> AvailableSubscriptions = new ()
    {
        new() { Id = Guid.NewGuid(), Name = "TestSubscription", Description = "Test Subscription Description", ProvisioningState = "Provisioning", MaxDeliveryAttempts = 3, DeadletterCount = 1, WebhookSecret = Guid.NewGuid().ToString() },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription2", Description = "Test Subscription Description 2", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 2, WebhookSecret = Guid.NewGuid().ToString() },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription3", Description = "Test Subscription Description 3", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 3, WebhookSecret = Guid.NewGuid().ToString() }
    };

    public readonly string ApiKey = Guid.NewGuid().ToString();

    public Mock<IOrganizationGateway> OrganizationServiceMock = new();
    public Mock<ILocalEnvironments> LocalEnvironmentsMock = new();
    public Mock<IEnvironmentGateway> EnvironmentGatewayMock = new();
    public Mock<IEventTypeGateway> EventTypeGatewayMock = new();
    public Mock<ITopicGateway> TopicGatewayMock = new();
    public Mock<ISubscriptionGateway> SubscriptionGatewayMock = new();

    public required IMediator Mediator { get; init; }

    public QalaCliBaseFixture()
    {
        InitializaLocalEnvironmentsMock();
        InitializeOrganizationGatewayMock();
        InitializeEnvironmentGatewayMock();
        InitializeEventTypeGatewayMock();
        InitializeTopicGatewayMock();
        InitializeSubscriptionGatewayMock();

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

    private void InitializeTopicGatewayMock()
    {
        AvailableTopics.ForEach(t => t.EventTypes = AvailableEventTypes);

        TopicGatewayMock.Setup(
            t => t.ListTopicsAsync())
                    .ReturnsAsync(AvailableTopics);
        
        TopicGatewayMock.Setup(
            t => t.GetTopicAsync(It.IsAny<string>()))
                    .ReturnsAsync((string name) => AvailableTopics.FirstOrDefault(t => t.Name == name));

        TopicGatewayMock.Setup(
            t => t.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>()))
                    .ReturnsAsync((string name, string description, List<Guid> eventTypeIds) => {
                        var newTopic = new Topic
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            Description = description,
                            ProvisioningState = "Provisioning",
                            EventTypes = AvailableEventTypes.Where(et => eventTypeIds.Contains(et.Id)).ToList()
                        };

                        AvailableTopics.Add(newTopic);

                        return newTopic;
                    });

        TopicGatewayMock.Setup(
            t => t.UpdateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>()))
                    .ReturnsAsync((string name, string description, List<Guid> eventTypeIds) => {
                        var topic = AvailableTopics.FirstOrDefault(t => t.Name == name);
                        if (topic != null)
                        {
                            topic.Description = description;
                            topic.EventTypes = AvailableEventTypes.Where(et => eventTypeIds.Contains(et.Id)).ToList();
                        }

                        return topic;
                    });
    }

    private void InitializeSubscriptionGatewayMock()
    {
        AvailableSubscriptions.ForEach(s => s.TopicName = AvailableTopics.First().Name);

        SubscriptionGatewayMock.Setup(
            s => s.ListSubscriptionsAsync(It.IsAny<string>()))
                    .ReturnsAsync(AvailableSubscriptions);
        
        SubscriptionGatewayMock.Setup(
            s => s.GetSubscriptionAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                    .ReturnsAsync((string topicName, Guid subscriptionId) => AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId));

        SubscriptionGatewayMock.Setup(
            s => s.CreateSubscriptionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>(), It.IsAny<int>()))
                    .ReturnsAsync((string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts) => {
                        var newSubscription = new Subscription
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            Description = description,
                            ProvisioningState = "Provisioning",
                            MaxDeliveryAttempts = maxDeliveryAttempts,
                            DeadletterCount = 0
                        };

                        AvailableSubscriptions.Add(newSubscription);

                        return newSubscription;
                    });

        SubscriptionGatewayMock.Setup(
            s => s.UpdateSubscriptionAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>(), It.IsAny<int>()))
                    .ReturnsAsync((string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts) => {
                        var subscription = AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription != null)
                        {
                            subscription.Name = name;
                            subscription.Description = description;
                            subscription.MaxDeliveryAttempts = maxDeliveryAttempts;
                        }

                        return subscription;
                    });

        SubscriptionGatewayMock.Setup(
            s => s.DeleteSubscriptionAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Callback((string topicName, Guid subscriptionId) => {
                        var subscription = AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription != null)
                        {
                            AvailableSubscriptions.Remove(subscription);
                        }
                    });

        SubscriptionGatewayMock.Setup(
            s => s.GetWebhookSecretAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                    .ReturnsAsync((string topicName, Guid subscriptionId) => AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId)?.WebhookSecret);

        SubscriptionGatewayMock.Setup(
            s => s.RotateWebhookSecretAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                    .ReturnsAsync((string topicName, Guid subscriptionId) => {
                        var subscription = AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription != null)
                        {
                            subscription.WebhookSecret = Guid.NewGuid().ToString();
                        }

                        return subscription?.WebhookSecret;
                    });
    }

    private static void InitializeCommandHandlers(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient<IRequestHandler<SetEnvironmentRequest, Either<SetEnvironmentErrorResponse, SetEnvironmentSuccessResponse>>, SetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<GetEnvironmentRequest, Either<GetEnvironmentErrorResponse, GetEnvironemntSuccessResponse>>, GetEnvironmentHandler>();
        services.AddTransient<IRequestHandler<CreateEnvironmentRequest, Either<CreateEnvironmentErrorResponse, CreateEnvironmentSuccessResponse>>, CreateEnvironmentHandler>();
        services.AddTransient<IRequestHandler<ListEventTypesRequest, Either<ListEventTypesErrorResponse, ListEventTypesSuccessResponse>>, ListEventTypesHandler>();
        services.AddTransient<IRequestHandler<GetEventTypeRequest, Either<GetEventTypeErrorResponse, GetEventTypeSuccessResponse>>, GetEventTypeHandler>();
        services.AddTransient<IRequestHandler<ListTopicRequest, Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>>, ListTopicsHandler>();
        services.AddTransient<IRequestHandler<GetTopicRequest, Either<GetTopicErrorResponse, GetTopicSuccessResponse>>, GetTopicHandler>();
        services.AddTransient<IRequestHandler<CreateTopicRequest, Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>, CreateTopicHandler>();
        services.AddTransient<IRequestHandler<UpdateTopicRequest, Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>>, UpdateTopicHandler>();
        services.AddTransient<IRequestHandler<ListSubscriptionsRequest, Either<ListSubscriptionsErrorResponse, ListSubscriptionsSuccessResponse>>, ListSubscriptionsHandler>();
        services.AddTransient<IRequestHandler<GetSubscriptionRequest, Either<GetSubscriptionErrorResponse, GetSubscriptionSuccessResponse>>, GetSubscriptionHandler>();
        services.AddTransient<IRequestHandler<CreateSubscriptionRequest, Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>, CreateSubscriptionHandler>();
        services.AddTransient<IRequestHandler<UpdateSubscriptionRequest, Either<UpdateSubscriptionErrorResponse, UpdateSubscriptionSuccessResponse>>, UpdateSubscriptionHandler>();
        services.AddTransient<IRequestHandler<DeleteSubscriptionRequest, Either<DeleteSubscriptionErrorResponse, DeleteSubscriptionSuccessResponse>>, DeleteSubscriptionHandler>();
        services.AddTransient<IRequestHandler<GetWebhookSecretRequest, Either<GetWebhookSecretErrorResponse, GetWebhookSecretSuccessResponse>>, GetWebhookSecretHandler>();
        services.AddTransient<IRequestHandler<RotateWebhookSecretRequest, Either<RotateWebhookSecretErrorResponse, RotateWebhookSecretSuccessResponse>>, RotateWebhookSecretHandler>();
    }

    private static void InitializeServices(IServiceCollection services)
    {
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IEventTypeService, EventTypeService>();
        services.AddTransient<ITopicService, TopicService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
    }

    private void InitializeDataServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalEnvironments>(LocalEnvironmentsMock.Object);
        services.AddSingleton<IOrganizationGateway>(OrganizationServiceMock.Object);
        services.AddSingleton<IEnvironmentGateway>(EnvironmentGatewayMock.Object);
        services.AddSingleton<IEventTypeGateway>(EventTypeGatewayMock.Object);
        services.AddSingleton<ITopicGateway>(TopicGatewayMock.Object);
        services.AddSingleton<ISubscriptionGateway>(SubscriptionGatewayMock.Object);
    }
}
