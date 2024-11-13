using System.Net;
using System.Net.Http.Json;
using Cli.Commands.Subscriptions;
using Cli.Models;
using Cli.Services.Interfaces;
using LanguageExt;

namespace Cli.Services;

internal class SubscriptionService(HttpClient publishingApiClient) : ISubscriptionService
{
    public async Task<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>> CreateSubscriptionTopicAsync(string name, string topicName, string description, Guid[] eventTypeIds, string webhookUrl, int maxDeliveryAttempts)
    {
        if (string.IsNullOrEmpty(topicName))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("No topic provided"));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("No description provided"));
        }

        if (eventTypeIds is null || eventTypeIds.Length == 0)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("No events provided"));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("No webhook URL provided"));
        }

        try
        {
            var response = await publishingApiClient.PostAsJsonAsync($"v1/topics/{topicName}/subscriptions", new
            {
                name,
                description,
                webhookUrl,
                eventTypeIds,
                maxDeliveryAttempts
            });

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                var errorDetails = error?.Errors != null ? string.Join("\n ", error.Errors.Select(e => e.Reason)) : "No details provided";
                return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse($"{error?.Title ?? "Unknown error"}:\n {errorDetails}"));
            }

            var data = await response.Content.ReadFromJsonAsync<Subscription>();

            if (data == null)
            {
                return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse("Failed to deserialize response"));
            }

            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionSuccessResponse(data.Name, data.TopicName, data.Description, data.EventTypes.Select(e => e.Id).ToArray(), data.WebhookUrl, data.MaxDeliveryAttempts));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<CreateSubscriptionErrorResponse, CreateSubscriptionSuccessResponse>>(new CreateSubscriptionErrorResponse(ex.Message));
        }
    }
}