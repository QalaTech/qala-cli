using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Data.Gateway.Interfaces;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Services;

public class SourceService(ISourceGateway sourceGateway) : ISourceService
{
    public async Task<Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>> CreateSourceAsync(string name, string description, SourceConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult(Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>.Left(new CreateSourceErrorResponse("Name is required")));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult(Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>.Left(new CreateSourceErrorResponse("Description is required")));
        }

        if (configuration == null)
        {
            return await Task.FromResult(Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>.Left(new CreateSourceErrorResponse("Configuration is required")));
        }

        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var source = await sourceGateway.CreateSourceAsync(name, description, SourceType.Http, JsonSerializer.Serialize<SourceConfiguration>(configuration, options));
        if (source == null)
        {
            return await Task.FromResult(Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>.Left(new CreateSourceErrorResponse("Failed to create source")));
        }

        return await Task.FromResult(Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>.Right(new CreateSourceSuccessResponse(source)));
    }

    public async Task<Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>> DeleteSourceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult(Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>.Left(new DeleteSourceErrorResponse("Source name is required")));
        }

        await sourceGateway.DeleteSourceAsync(name);
        return await Task.FromResult(Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>.Right(new DeleteSourceSuccessResponse()));
    }

    public async Task<Either<GetSourceErrorResponse, GetSourceSuccessResponse>> GetSourceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult(Either<GetSourceErrorResponse, GetSourceSuccessResponse>.Left(new GetSourceErrorResponse("Source name is required")));
        }

        var source = await sourceGateway.GetSourceAsync(name);
        if (source == null)
        {
            return await Task.FromResult(Either<GetSourceErrorResponse, GetSourceSuccessResponse>.Left(new GetSourceErrorResponse("Source not found")));
        }

        return await Task.FromResult(Either<GetSourceErrorResponse, GetSourceSuccessResponse>.Right(new GetSourceSuccessResponse(source)));
    }

    public async Task<Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>> ListSourcesAsync()
    {
        var sources = await sourceGateway.ListSourcesAsync();
        return await Task.FromResult(Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>.Right(new ListSourcesSuccessResponse(sources)));
    }

    public async Task<Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>> UpdateSourceAsync(string sourceName, string name, string description, SourceConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
        {
            return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Left(new UpdateSourceErrorResponse("Source name is required")));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Left(new UpdateSourceErrorResponse("Name is required")));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Left(new UpdateSourceErrorResponse("Description is required")));
        }

        if (configuration == null)
        {
            return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Left(new UpdateSourceErrorResponse("Configuration is required")));
        }

        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var source = await sourceGateway.UpdateSourceAsync(sourceName, name, description, SourceType.Http, JsonSerializer.Serialize<SourceConfiguration>(configuration, options));
        if (source == null)
        {
            return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Left(new UpdateSourceErrorResponse("Failed to update source")));
        }

        return await Task.FromResult(Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>.Right(new UpdateSourceSuccessResponse(source)));
    }
}