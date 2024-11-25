using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface ISubscriptionGateway
{
    Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string topicName);
    Task<Subscription> GetSubscriptionAsync(string topicName, Guid subscriptionId);
    Task<Subscription> CreateSubscriptionAsync(string topicName, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts);
    Task<Subscription> UpdateSubscriptionAsync(string topicName, Guid subscriptionId, string name, string description, string webhookUrl, List<Guid> eventTypeIds, int maxDeliveryAttempts);
}