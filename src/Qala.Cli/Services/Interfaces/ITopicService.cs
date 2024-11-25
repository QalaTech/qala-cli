using System;
using LanguageExt;
using Qala.Cli.Commands.Topics;

namespace Qala.Cli.Services.Interfaces;

public interface ITopicService
{
    Task<Either<ListTopicsErrorResponse, ListTopicsSuccessResponse>> ListTopicsAsync();
    Task<Either<GetTopicErrorResponse, GetTopicSuccessResponse>> GetTopicAsync(string name);
    Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> CreateTopicAsync(string name, string description, List<Guid> eventTypeIds);
    Task<Either<UpdateTopicErrorResponse, UpdateTopicSuccessResponse>> UpdateTopicAsync(string name, string description, List<Guid> eventTypeIds);
}
