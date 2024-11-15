using Qala.Cli.Commands.Create.Topics;
using LanguageExt;

namespace Qala.Cli.Services.Interfaces;

internal interface ITopicService
{
    public Task<Either<CreateTopicErrorResponse, CreateTopicSuccessResponse>> CreateTopicAsync(string name, string description, Guid[] events);
}