using System.Text.RegularExpressions;
using LanguageExt;
using Qala.Cli.Commands.SubscriberGroups;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;
using Qala.Cli.Utils;

namespace Qala.Cli.Services;

public class SubscriberGroupService(ISubscriberGroupGateway subscriberGroupGateway) : ISubscriberGroupService
{
    public async Task<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>> CreateSubscriberGroupAsync(string name, string description, List<string> topics, string audience)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse("Subscriber Group name is required"));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse("Subscriber Group description is required"));
        }

        if (topics == null || topics.Count == 0)
        {
            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse("At least one topic is required"));
        }

        if (!string.IsNullOrWhiteSpace(audience) && !Regex.Match(audience, ValidationHelper.AudiencesRegex, RegexOptions.IgnoreCase).Success)
        {
            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse("Audience must only contain alphanumerical values (A-Z, a-z, 0-9)"));
        }

        try
        {
            var permissions = new List<Permission>();
            foreach (var topic in topics)
            {
                permissions.Add(new Permission()
                {
                    PermissionType = "Topic:Subscribe",
                    ResourceType = "Topic",
                    ResourceId = topic
                });
            }

            var response = await subscriberGroupGateway.CreateSubscriberGroupAsync(name, description, permissions, audience);
            if (response == null)
            {
                return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse("Failed to create Subscriber Group"));
            }

            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupSuccessResponse(response));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<CreateSubscriberGroupErrorResponse, CreateSubscriberGroupSuccessResponse>>(new CreateSubscriberGroupErrorResponse(ex.Message));

        }
    }

    public async Task<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>> DeleteSubscriberGroupAsync(string subscriberGroupName)
    {
        if (string.IsNullOrWhiteSpace(subscriberGroupName))
        {
            return await Task.FromResult<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>(new DeleteSubscriberGroupErrorResponse("Subscriber Group name is required"));
        }

        var subscriberGroups = await subscriberGroupGateway.ListSubscriberGroupsAsync();
        if (subscriberGroups == null || subscriberGroups.Count == 0)
        {
            return await Task.FromResult<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>(new DeleteSubscriberGroupErrorResponse("Subscriber Group not found"));
        }

        var subscriberGroup = subscriberGroups.FirstOrDefault(x => x.Name == subscriberGroupName);
        if (subscriberGroup == null)
        {
            return await Task.FromResult<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>(new DeleteSubscriberGroupErrorResponse("Subscriber Group not found"));
        }


        try
        {
            await subscriberGroupGateway.DeleteSubscriberGroupAsync(subscriberGroup.Key);
            return await Task.FromResult<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>(new DeleteSubscriberGroupSuccessResponse());
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<DeleteSubscriberGroupErrorResponse, DeleteSubscriberGroupSuccessResponse>>(new DeleteSubscriberGroupErrorResponse(ex.Message));
        }
    }

    public async Task<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>> GetSubscriberGroupAsync(string subscriberGroupName)
    {
        if (string.IsNullOrWhiteSpace(subscriberGroupName))
        {
            return await Task.FromResult<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>(new GetSubscriberGroupErrorResponse("Subscriber Group name is required"));
        }

        var subscriberGroups = await subscriberGroupGateway.ListSubscriberGroupsAsync();
        if (subscriberGroups == null || subscriberGroups.Count == 0)
        {
            return await Task.FromResult<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>(new GetSubscriberGroupErrorResponse("Subscriber Group not found"));
        }

        var subscriberGroup = subscriberGroups.FirstOrDefault(x => x.Name == subscriberGroupName);
        if (subscriberGroup == null)
        {
            return await Task.FromResult<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>(new GetSubscriberGroupErrorResponse("Subscriber Group not found"));
        }

        return await Task.FromResult<Either<GetSubscriberGroupErrorResponse, GetSubscriberGroupSuccessResponse>>(new GetSubscriberGroupSuccessResponse(subscriberGroup));
    }

    public async Task<Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>> ListSubscriberGroupsAsync()
    {
        var subscriberGroups = await subscriberGroupGateway.ListSubscriberGroupsAsync();

        return await Task.FromResult<Either<ListSubscriberGroupsErrorResponse, ListSubscriberGroupsSuccessResponse>>(new ListSubscriberGroupsSuccessResponse(subscriberGroups));
    }

    public async Task<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>> UpdateSubscriberGroupAsync(string subscriberGroupName, string? newName, string? description, List<string>? topics, string? audience)
    {
        if (string.IsNullOrWhiteSpace(subscriberGroupName))
        {
            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse("Subscriber Group name is required"));
        }

        if (!string.IsNullOrWhiteSpace(audience) && !Regex.Match(audience, ValidationHelper.AudiencesRegex, RegexOptions.IgnoreCase).Success)
        {
            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse("Audience must only contain alphanumerical values (A-Z, a-z, 0-9)"));
        }

        var subscriberGroups = await subscriberGroupGateway.ListSubscriberGroupsAsync();
        if (subscriberGroups == null || subscriberGroups.Count == 0)
        {
            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse("Subscriber Group not found"));
        }

        var subscriberGroup = subscriberGroups.FirstOrDefault(x => x.Name == subscriberGroupName);
        if (subscriberGroup == null)
        {
            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse("Subscriber Group not found"));
        }

        if (!string.IsNullOrEmpty(newName))
        {
            subscriberGroup.Name = newName;
        }

        if (!string.IsNullOrEmpty(description))
        {
            subscriberGroup.Description = description;
        }

        if (!string.IsNullOrEmpty(audience))
        {
            subscriberGroup.Audience = audience;
        }

        if (topics != null && topics.Count > 0)
        {
            subscriberGroup.AvailablePermissions = new List<Permission>();
            foreach (var topic in topics)
            {
                subscriberGroup.AvailablePermissions.Add(new Permission()
                {
                    PermissionType = "Topic:Subscribe",
                    ResourceType = "Topic",
                    ResourceId = topic
                });
            }
        }

        try
        {
            var response = await subscriberGroupGateway.UpdateSubscriberGroupAsync(subscriberGroup.Key, subscriberGroup.Name, subscriberGroup.Description, subscriberGroup.AvailablePermissions, subscriberGroup.Audience);
            if (response == null)
            {
                return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse("Failed to update Subscriber Group"));
            }

            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupSuccessResponse(response));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<UpdateSubscriberGroupErrorResponse, UpdateSubscriberGroupSuccessResponse>>(new UpdateSubscriberGroupErrorResponse(ex.Message));
        }
    }
}