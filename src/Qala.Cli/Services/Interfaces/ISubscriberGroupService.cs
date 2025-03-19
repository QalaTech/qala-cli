using LanguageExt;
using Qala.Cli.Commands.SubscriberGroups;

namespace Qala.Cli.Services.Interfaces;

public interface ISubscriberGroupService
{
    Task<Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>> ListSubscriberGroupsAsync();
    Task<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>> GetSubscriberGroupAsync(string subscriberGroupName);
    Task<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>> CreateSubscriberGroupAsync(string name, string description, List<string> topics, string audience);
    Task<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>> UpdateSubscriberGroupAsync(string subscriberGroupName, string? newName, string? description, List<string>? topics, string? audience);
    Task<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>> DeleteSubscriberGroupAsync(string subscriberGroupName);
}