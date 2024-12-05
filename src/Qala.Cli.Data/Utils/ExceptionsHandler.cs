using System.Net.Http.Json;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Utils;

public static class ExceptionsHandler
{
    public static async Task ThrowExceptionWithProblemDetails(HttpResponseMessage response, string message)
    {
        try
        {
            var data = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            if (data != null && data.Errors != null)
            {
                throw new Exception(message + string.Join(", ", data.Errors.Select(x => x.Reason)));
            }
        }
        catch
        {
            throw new Exception(message);
        }

        throw new Exception(message);
    }
}