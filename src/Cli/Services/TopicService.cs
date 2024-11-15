using System.Net.Http.Json;
using System.Net;
using Cli.Models;
using Cli.Services.Interfaces;
using LanguageExt;
using Cli.Commands.Create.Topics;

namespace Qala.Cli.Services;

internal class TopicService(HttpClient publishingApiClient) : ITopicService
{
    public async Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> CreateTopicAsync(string name, string description, Guid[] events)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("No description provided"));
        }

        if (events is null || events.Length == 0)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("No events provided"));
        }

        try
        {
            var response = await publishingApiClient.PostAsJsonAsync($"v1/topics", new
            {
                name,
                description,
                eventTypeIds=events
            });

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                var errorDetails = error?.Errors != null ? string.Join("\n ", error.Errors.Select(e => e.Reason)) : "No details provided";
                return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse($"{error?.Title ?? "Unknown error"}:\n {errorDetails}"));
            }

            var data = await response.Content.ReadFromJsonAsync<Topic>();

            if (data == null)
            {
                return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse("Failed to deserialize response"));
            }
            
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicSuccessResponse(data.Name, data.Description, data.EventTypes.Select(e => e.Id).ToArray()));
        }
        catch (Exception ex)
        {
            return await Task.FromResult<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>>(new CreateTopicErrorResponse(ex.Message));
        }
    }
}