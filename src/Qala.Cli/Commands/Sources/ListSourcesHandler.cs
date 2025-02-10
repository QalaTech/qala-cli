using LanguageExt;
using MediatR;
using Qala.Cli.Data.Models;
using Qala.Cli.Services.Interfaces;

namespace Qala.Cli.Commands.Sources;

public record ListSourcesSuccessResponse(IEnumerable<Source> Sources);
public record ListSourcesErrorResponse(string Message);
public record ListSourcesRequest : IRequest<Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>>;

public class ListSourcesHandler(ISourceService sourceService)
    : IRequestHandler<ListSourcesRequest, Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>>
{
    public async Task<Either<ListSourcesErrorResponse, ListSourcesSuccessResponse>> Handle(ListSourcesRequest request, CancellationToken cancellationToken)
        => await sourceService.ListSourcesAsync()
            .ToAsync()
            .Case switch
        {
            ListSourcesSuccessResponse success => success,
            ListSourcesErrorResponse error => error,
            _ => throw new NotImplementedException()
        };
}