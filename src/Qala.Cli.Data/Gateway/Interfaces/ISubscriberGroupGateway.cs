using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface ISubscriberGroupGateway
{
    Task<List<SubscriberGroupPrincipal>> ListSubscriberGroupsAsync();
    Task<SubscriberGroupPrincipal> GetSubscriberGroupAsync(Guid subscriberGroupId);
    Task<SubscriberGroupPrincipal?> CreateSubscriberGroupAsync(string name, string description, List<Permission> permissions, string audience);
    Task UpdateSubscriberGroupAsync(Guid subscriberGroupId, string? name, string? description, List<Permission>? permissions, string? audience);
    Task DeleteSubscriberGroupAsync(Guid subscriberGroupId);
}