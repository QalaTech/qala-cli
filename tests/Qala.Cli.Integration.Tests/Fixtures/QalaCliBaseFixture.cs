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
using Qala.Cli.Commands.Sources;
using System.Text.Json;
using Qala.Cli.Data.Utils;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Qala.Cli.Configurations;
using Spectre.Console.Cli;
using Qala.Cli.Commands.SubscriberGroups;

namespace Qala.Cli.Integration.Tests.Fixtures;

public class QalaCliBaseFixture : IDisposable
{
    public readonly List<Data.Models.Environment> AvailableEnvironments =
    [
        new() { Id = Guid.Parse("55908371-b629-40b5-97bc-d6064cf8d3cd"), Name = "TestEnv", Region = "eu", EnvironmentType = "development", IsSchemaValidationEnabled = false },
        new() { Id = Guid.Parse("2ac732a9-97d9-4218-80b7-4e974be074fe"), Name = "TestEnv2", Region = "eu", EnvironmentType = "development", IsSchemaValidationEnabled = true  },
        new() { Id = Guid.Parse("ba1887bb-89e2-4dac-b3f9-03b4e519b59d"), Name = "TestEnv3", Region = "eu", EnvironmentType = "production", IsSchemaValidationEnabled = true  }
    ];

    public readonly List<EventType> AvailableEventTypes =
    [
        new() { Id = Guid.NewGuid(), Type = "TestEvent1", Description = "Test Event Description", Schema="{\"name\":\"Test\"}", ContentType="application/json", Encoding="utf-8", Categories = ["cat1", "cat2"] },
        new() { Id = Guid.NewGuid(), Type = "TestEvent2", Description = "Test Event Description 2", Schema="{\"name\":\"Test2\"}", ContentType="application/json", Encoding="utf-8", Categories = ["cat3", "cat4"] },
        new() { Id = Guid.NewGuid(), Type = "TestEvent3", Description = "Test Event Description 3", Schema="{\"name\":\"Test3\"}", ContentType="application/json", Encoding="utf-8", Categories = ["cat5", "cat6"] }
    ];

    public List<Topic> AvailableTopics =
    [
        new() { Id = Guid.NewGuid(), Name = "TestTopic", Description = "Test Topic Description", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic2", Description = "Test Topic Description 2", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic3", Description = "Test Topic Description 3", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic4", Description = "Test Topic Description 4", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic5", Description = "Test Topic Description 5", ProvisioningState = "Provisioning" },
        new() { Id = Guid.NewGuid(), Name = "TestTopic6", Description = "Test Topic Description 6", ProvisioningState = "Provisioning" }
    ];

    public List<Source> AvailableSources =
    [
        new () { SourceId = Guid.NewGuid(), Name = "TestSource", Description = "Test Source Description", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new NoAuthenticationScheme(), WhitelistedIpRanges = ["102.0.0.1"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource2", Description = "Test Source Description 2", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.PUT, Data.Models.HttpMethod.POST], AuthenticationScheme = new BasicAuthenticationScheme() { PasswordReference = "password_ref", Username = "username" }, WhitelistedIpRanges = ["127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource3", Description = "Test Source Description 3", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.DELETE, Data.Models.HttpMethod.POST], AuthenticationScheme = new ApiKeyAuthenticationScheme() { ApiKeyName = "key", ApiKeyValueReference = "value" }, WhitelistedIpRanges = ["102.0.0.1/24"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource4", Description = "Test Source Description 4", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.RSA, PublicKey = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource5", Description = "Test Source Description 5", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource6", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource7", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource8", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource9", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource10", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}},
        new () { SourceId = Guid.NewGuid(), Name = "TestSource11", Description = "Test Source Description 6", SourceType = SourceType.Http, Configuration = new SourceConfiguration { AllowedHttpMethods = [Data.Models.HttpMethod.GET, Data.Models.HttpMethod.POST], AuthenticationScheme = new JwtAuthenticationScheme() { Algorithm = Algorithm.HSA, Secret = "public_key" }, WhitelistedIpRanges = ["102.0.0.1/24, 127.0.0.1, 127.0.0.2"]}}
    ];

    public List<Subscription> AvailableSubscriptions =
    [
        new() { Id = Guid.NewGuid(), Name = "TestSubscription", Description = "Test Subscription Description", ProvisioningState = "Provisioning", MaxDeliveryAttempts = 3, DeadletterCount = 1, WebhookSecret = Guid.NewGuid().ToString(), Audience = "audience" },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription2", Description = "Test Subscription Description 2", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 2, WebhookSecret = Guid.NewGuid().ToString(), Audience = "audience2" },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription3", Description = "Test Subscription Description 3", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 3, WebhookSecret = Guid.NewGuid().ToString(), Audience = "audience3" },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription4", Description = "Test Subscription Description 4", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 4, WebhookSecret = Guid.NewGuid().ToString(), Audience = "audience4" },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription5", Description = "Test Subscription Description 5", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 5, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription6", Description = "Test Subscription Description 6", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 6, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription7", Description = "Test Subscription Description 7", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 7, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription8", Description = "Test Subscription Description 8", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 8, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription9", Description = "Test Subscription Description 9", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 9, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription10", Description = "Test Subscription Description 10", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 10, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription11", Description = "Test Subscription Description 11", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 11, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription12", Description = "Test Subscription Description 12", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 12, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription13", Description = "Test Subscription Description 13", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 13, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription14", Description = "Test Subscription Description 14", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 14, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription15", Description = "Test Subscription Description 15", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 15, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription16", Description = "Test Subscription Description 16", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 16, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription17", Description = "Test Subscription Description 17", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 17, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription18", Description = "Test Subscription Description 18", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 18, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription19", Description = "Test Subscription Description 19", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 19, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription20", Description = "Test Subscription Description 20", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 20, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription21", Description = "Test Subscription Description 21", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 21, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription22", Description = "Test Subscription Description 22", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 22, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription23", Description = "Test Subscription Description 23", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 22, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription24", Description = "Test Subscription Description 24", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 22, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription25", Description = "Test Subscription Description 25", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 22, WebhookSecret = Guid.NewGuid().ToString(), Audience = null },
        new() { Id = Guid.NewGuid(), Name = "TestSubscription26", Description = "Test Subscription Description 26", ProvisioningState = "Provisioned", MaxDeliveryAttempts = 3, DeadletterCount = 22, WebhookSecret = Guid.NewGuid().ToString(), Audience = "audience26" }
    ];

    public List<string> AvailableApiKeys =
    [
        "297b5ae0-37c8-4419-b1f4-41f3d998d78e",
        "3f539e05-f8ff-45e0-b904-43cbdd7314f3",
        "4f3628c4-70a4-4fe8-b23e-51eec93a90ab"
    ];

    public List<SubscriberGroupPrincipal> AvailableSubscriberGroups =
    [
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup", Description = "Test Subscriber Group Description", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic" } }, Audience = "audience" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup2", Description = "Test Subscriber Group Description 2", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic2" } }, Audience = "audience2" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup3", Description = "Test Subscriber Group Description 3", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic3" } }, Audience = "audience3" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup4", Description = "Test Subscriber Group Description 4", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic4" } }, Audience = "audience4" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup5", Description = "Test Subscriber Group Description 5", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic5" } }, Audience = "audience5" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup6", Description = "Test Subscriber Group Description 6", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic6" } }, Audience = "audience6" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup7", Description = "Test Subscriber Group Description 7", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic7" } }, Audience = "audience7" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup8", Description = "Test Subscriber Group Description 8", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic8" } }, Audience = "audience8" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup9", Description = "Test Subscriber Group Description 9", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic9" } }, Audience = "audience9" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup10", Description = "Test Subscriber Group Description 10", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic10" } }, Audience = "audience10" },
        new() { Key = Guid.NewGuid(), Name = "TestSubscriberGroup11", Description = "Test Subscriber Group Description 11", AvailablePermissions = new List<Permission> { new() { PermissionType = "Topic:Subscribe", ResourceType = "Topic", ResourceId = "TestTopic11" } }, Audience = "audience11" },
    ];

    public Mock<IOrganizationGateway> OrganizationServiceMock = new();
    public Mock<IEnvironmentGateway> EnvironmentGatewayMock = new();
    public Mock<IEventTypeGateway> EventTypeGatewayMock = new();
    public Mock<ITopicGateway> TopicGatewayMock = new();
    public Mock<ISubscriptionGateway> SubscriptionGatewayMock = new();
    public Mock<ISourceGateway> SourceGatewayMock = new();
    public Mock<ISubscriberGroupGateway> SubscriberGroupGatewayMock = new();

    public CommandApp App { get; set; }

    public required IMediator Mediator { get; init; }

    public QalaCliBaseFixture()
    {
        InitializeOrganizationGatewayMock();
        InitializeEnvironmentGatewayMock();
        InitializeEventTypeGatewayMock();
        InitializeTopicGatewayMock();
        InitializeSourcesGatewayMock();
        InitializeSubscriptionGatewayMock();
        InitializeSubscriberGroupGatewayMock();

        var services = new ServiceCollection();
        InitializeDataServices(services);
        InitializeCommandHandlers(services);
        InitializeServices(services);

        var registrar = new TypeRegistrar(services);
        App = new CommandApp(registrar);

        var serviceProvider = services.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public static void InitializeEnvironmentVariables(string? apiKey = null, string? envId = null, string? authToken = null)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            System.Environment.SetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], apiKey, EnvironmentVariableTarget.User);
            System.Environment.SetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], envId, EnvironmentVariableTarget.User);
            System.Environment.SetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], authToken, EnvironmentVariableTarget.User);
        }

        UnsetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);
        UnsetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);
        UnsetEnvironmentVariable(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN]);

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            SetEnvironmentVariableNonWindows(Constants.LocalVariable[LocalVariableType.QALA_API_KEY], apiKey);
        }

        if (!string.IsNullOrWhiteSpace(envId))
        {
            SetEnvironmentVariableNonWindows(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID], envId);
        }

        if (!string.IsNullOrWhiteSpace(authToken))
        {
            SetEnvironmentVariableNonWindows(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], authToken);
        }
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
                    .ReturnsAsync((Data.Models.Environment inputEnvironment) =>
                    {
                        var newEnvironment = new Data.Models.Environment
                        {
                            Id = new Guid("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                            Name = "NewlyCreatedTestEnv",
                            Region = "newly-region",
                            EnvironmentType = "newly-env-type",
                            IsSchemaValidationEnabled = inputEnvironment.IsSchemaValidationEnabled
                        };

                        AvailableEnvironments.Add(newEnvironment);

                        return newEnvironment;
                    });

        EnvironmentGatewayMock.Setup(
            e => e.UpdateEnvironmentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .ReturnsAsync((Guid environmentId, string environmentName, bool disableSchemaValidation) =>
                    {
                        var environment = AvailableEnvironments.FirstOrDefault(e => e.Id == environmentId);
                        if (environment != null)
                        {
                            environment.Name = environmentName;
                            environment.IsSchemaValidationEnabled = !disableSchemaValidation;
                        }

                        return environment;
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
                    .ReturnsAsync((string name, string description, List<Guid> eventTypeIds) =>
                    {
                        var newTopic = new Topic
                        {
                            Id = new Guid("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
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
                    .ReturnsAsync((string name, string description, List<Guid> eventTypeIds) =>
                    {
                        var topic = AvailableTopics.FirstOrDefault(t => t.Name == name);
                        if (topic != null)
                        {
                            topic.Name = name;
                            topic.Description = description;
                            topic.ProvisioningState = "Provisioning";
                            topic.EventTypes = AvailableEventTypes.Where(et => eventTypeIds.Contains(et.Id)).ToList();
                        }

                        return topic;
                    });
    }

    private void InitializeSourcesGatewayMock()
    {
        SourceGatewayMock.Setup(
            s => s.ListSourcesAsync())
                    .ReturnsAsync(AvailableSources);

        SourceGatewayMock.Setup(
            s => s.GetSourceAsync(It.IsAny<string>()))
                    .ReturnsAsync((string name) => AvailableSources.FirstOrDefault(t => t.Name == name));

        SourceGatewayMock.Setup(
            s => s.DeleteSourceAsync(It.IsAny<string>()))
                    .Callback((string sourceName) =>
                    {
                        var source = AvailableSources.FirstOrDefault(s => s.Name == sourceName);
                        if (source != null)
                        {
                            AvailableSources.Remove(source);
                        }
                    });

        SourceGatewayMock.Setup(
            s => s.CreateSourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SourceType>(), It.IsAny<string>()))
                    .ReturnsAsync((string name, string description, SourceType sourceType, string configuration) =>
                    {
                        var options = new JsonSerializerOptions
                        {
                            Converters = { new AuthenticationSchemeConverter(), new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                        };

                        var sourceConfiguration = JsonSerializer.Deserialize<SourceConfiguration>(configuration, options);

                        var newSource = new Source
                        {
                            SourceId = new Guid("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                            Name = name,
                            Description = description,
                            SourceType = sourceType,
                            Configuration = sourceConfiguration ?? throw new ArgumentNullException(nameof(sourceConfiguration)),
                        };

                        AvailableSources.Add(newSource);

                        return newSource;
                    });

        SourceGatewayMock.Setup(
            s => s.UpdateSourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SourceType>(), It.IsAny<string>()))
                    .ReturnsAsync((string sourceName, string name, string description, SourceType sourceType, string configuration) =>
                    {
                        var options = new JsonSerializerOptions
                        {
                            Converters = { new AuthenticationSchemeConverter(), new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                        };

                        var source = AvailableSources.FirstOrDefault(t => t.Name == name);

                        var sourceConfiguration = JsonSerializer.Deserialize<SourceConfiguration>(configuration, options);

                        if (source != null)
                        {
                            source.Name = name;
                            source.Description = description;
                            source.Configuration = sourceConfiguration ?? throw new ArgumentNullException(nameof(sourceConfiguration));
                        }

                        return source;
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
            s => s.CreateSubscriptionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync((string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts, string audience) =>
                    {
                        var newSubscription = new Subscription
                        {
                            Id = new Guid("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                            Name = name,
                            Description = description,
                            WebhookUrl = webhookUrl,
                            EventTypes = AvailableEventTypes,
                            ProvisioningState = "Provisioning",
                            MaxDeliveryAttempts = maxDeliveryAttempts,
                            DeadletterCount = 0,
                            Audience = audience
                        };

                        AvailableSubscriptions.Add(newSubscription);

                        return newSubscription;
                    });

        SubscriptionGatewayMock.Setup(
            s => s.UpdateSubscriptionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Guid>>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync((string topicType, string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts, string audience) =>
                    {
                        var subscription = AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription != null)
                        {
                            subscription.Id = subscriptionId;
                            subscription.Name = name;
                            subscription.Description = description;
                            subscription.WebhookUrl = webhookUrl;
                            subscription.EventTypes = AvailableEventTypes.Where(et => eventTypeIds.Contains(et.Id)).ToList();
                            subscription.MaxDeliveryAttempts = maxDeliveryAttempts;
                            subscription.Audience = audience;
                        }

                        return subscription;
                    });

        SubscriptionGatewayMock.Setup(
            s => s.DeleteSubscriptionAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Callback((string topicName, Guid subscriptionId) =>
                    {
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
                    .ReturnsAsync((string topicName, Guid subscriptionId) =>
                    {
                        var subscription = AvailableSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription != null)
                        {
                            subscription.WebhookSecret = "80ef03bb-f5a7-4c81-addf-38e2b360bff5";
                        }

                        return subscription?.WebhookSecret;
                    });
    }

    private void InitializeSubscriberGroupGatewayMock()
    {
        SubscriberGroupGatewayMock.Setup(
            s => s.ListSubscriberGroupsAsync())
                    .ReturnsAsync(AvailableSubscriberGroups);

        SubscriberGroupGatewayMock.Setup(
            s => s.CreateSubscriberGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Permission>>(), It.IsAny<string>()))
                    .ReturnsAsync((string name, string description, List<Permission> permissions, string audience) =>
                    {
                        var newSubscriberGroup = new SubscriberGroupPrincipal
                        {
                            Key = new Guid("60ef03bb-f5a7-4c81-addf-38e2b360bff5"),
                            Name = name,
                            Description = description,
                            AvailablePermissions = permissions,
                            Audience = audience
                        };

                        AvailableSubscriberGroups.Add(newSubscriberGroup);

                        return newSubscriberGroup;
                    });

        SubscriberGroupGatewayMock.Setup(
            s => s.GetSubscriberGroupAsync(It.IsAny<Guid>()))
                    .ReturnsAsync((Guid id) => AvailableSubscriberGroups.FirstOrDefault(s => s.Key == id));

        SubscriberGroupGatewayMock.Setup(
            s => s.UpdateSubscriberGroupAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<Permission>>(), It.IsAny<string>()))
                    .Callback((Guid id, string name, string description, List<Permission> permissions, string audience) =>
                    {
                        var subscriberGroup = AvailableSubscriberGroups.FirstOrDefault(s => s.Key == id);
                        if (subscriberGroup != null)
                        {
                            subscriberGroup.Name = name;
                            subscriberGroup.Description = description;
                            subscriberGroup.AvailablePermissions = permissions;
                            subscriberGroup.Audience = audience;
                        }

                        AvailableSubscriberGroups.RemoveAll(s => s.Key == id);
                        AvailableSubscriberGroups.Add(subscriberGroup);
                    });

        SubscriberGroupGatewayMock.Setup(
            s => s.DeleteSubscriberGroupAsync(It.IsAny<Guid>()))
                    .Callback((Guid id) =>
                    {
                        var subscriberGroup = AvailableSubscriberGroups.FirstOrDefault(s => s.Key == id);
                        if (subscriberGroup != null)
                        {
                            AvailableSubscriberGroups.Remove(subscriberGroup);
                        }
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
        services.AddTransient<IRequestHandler<UpdateEnvironmentRequest, Either<UpdateEnvironmentErrorResponse, UpdateEnvironmentSuccessResponse>>, UpdateEnvironmentHandler>();
        services.AddTransient<IRequestHandler<CreateSourceRequest, Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>>, CreateSourceHandler>();
        services.AddTransient<IRequestHandler<ListSourcesRequest, Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>>, ListSourcesHandler>();
        services.AddTransient<IRequestHandler<GetSourceRequest, Either<GetSourceErrorResponse, GetSourceSuccessResponse>>, GetSourceHandler>();
        services.AddTransient<IRequestHandler<UpdateSourceRequest, Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>>, UpdateSourceHandler>();
        services.AddTransient<IRequestHandler<DeleteSourceRequest, Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>>, DeleteSourceHandler>();
        services.AddTransient<IRequestHandler<ListSubscriberGroupsRequest, Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>>, ListSubscriberGroupsHandler>();
        services.AddTransient<IRequestHandler<CreateSubscriberGroupRequest, Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>, CreateSubscriberGroupHandler>();
        services.AddTransient<IRequestHandler<UpdateSubscriberGroupRequest, Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>, UpdateSubscriberGroupHandler>();
        services.AddTransient<IRequestHandler<DeleteSubscriberGroupRequest, Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>, DeleteSubscriberGroupHandler>();
        services.AddTransient<IRequestHandler<GetSubscriberGroupRequest, Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>, GetSubscriberGroupHandler>();
    }

    private static void InitializeServices(IServiceCollection services)
    {
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IRequestHandler<ConfigRequest, Either<ConfigErrorResponse, ConfigSuccessResponse>>, ConfigHandler>();
        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IEventTypeService, EventTypeService>();
        services.AddTransient<ITopicService, TopicService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddTransient<ISourceService, SourceService>();
        services.AddTransient<ISubscriberGroupService, SubscriberGroupService>();
    }

    private void InitializeDataServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalEnvironments, LocalEnvironments>();
        services.AddSingleton<IOrganizationGateway>(OrganizationServiceMock.Object);
        services.AddSingleton<IEnvironmentGateway>(EnvironmentGatewayMock.Object);
        services.AddSingleton<IEventTypeGateway>(EventTypeGatewayMock.Object);
        services.AddSingleton<ITopicGateway>(TopicGatewayMock.Object);
        services.AddSingleton<ISubscriptionGateway>(SubscriptionGatewayMock.Object);
        services.AddSingleton<ISourceGateway>(SourceGatewayMock.Object);
        services.AddSingleton<ISubscriberGroupGateway>(SubscriberGroupGatewayMock.Object);
    }

    private static void SetEnvironmentVariableNonWindows(string variable, string value)
    {
        string shellConfigFile = GetShellConfigFile();
        string[] lines = File.ReadAllLines(shellConfigFile);
        bool variableFound = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith($"export {variable}="))
            {
                variableFound = true;
                if (value != null)
                {
                    lines[i] = $"export {variable}={value}";
                }
                else
                {
                    lines[i] = string.Empty; // Mark for removal
                }
            }
        }

        if (!variableFound && value != null)
        {
            Array.Resize(ref lines, lines.Length + 1);
            lines[^1] = $"export {variable}={value}";
        }

        File.WriteAllLines(shellConfigFile, lines.Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    private static void UnsetEnvironmentVariable(string variable)
    {
        string shellConfigFile = GetShellConfigFile();
        string[] lines = File.ReadAllLines(shellConfigFile);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith($"export {variable}="))
            {
                lines[i] = string.Empty; // Mark for removal
            }
        }

        File.WriteAllLines(shellConfigFile, lines.Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    private static string GetShellConfigFile()
    {
        string homeDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string shell = System.Environment.GetEnvironmentVariable("SHELL") ?? string.Empty;

        if (shell != null && shell.Contains("zsh"))
        {
            return Path.Combine(homeDirectory, ".zshrc");
        }
        else
        {
            return Path.Combine(homeDirectory, ".bash_profile");
        }
    }
}
