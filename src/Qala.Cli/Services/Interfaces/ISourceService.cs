using LanguageExt;
using Qala.Cli.Commands.Sources;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Services.Interfaces;

public interface ISourceService
{
    Task<Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>> ListSourcesAsync();
    Task<Either<GetSourceErrorResponse, GetSourceSuccessResponse>> GetSourceAsync(string name);
    Task<Either<CreateSourceErrorResponse, CreateSourceSuccessResponse>> CreateSourceAsync(string name, string description, SourceConfiguration configuration);
    Task<Either<UpdateSourceErrorResponse, UpdateSourceSuccessResponse>> UpdateSourceAsync(string sourceName, string name, string description, SourceConfiguration configuration);
    Task<Either<DeleteSourceErrorResponse, DeleteSourceSuccessResponse>> DeleteSourceAsync(string name);
}