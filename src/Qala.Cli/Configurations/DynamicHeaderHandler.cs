using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Utils;

namespace Qala.Cli.Configurations;

public class DynamicHeaderHandler(ILocalEnvironments localEnvironments) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_API_KEY]);

        if (string.IsNullOrEmpty(key))
        {
            var token = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN]);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        else
        {
            if (request.Headers.Contains("x-auth-token"))
            {
                request.Headers.Remove("x-auth-token");
            }

            request.Headers.Add("x-auth-token", key);
        }

        var environmentId = localEnvironments.GetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_ENVIRONMENT_ID]);
        if (!string.IsNullOrEmpty(environmentId))
        {
            if (request.Headers.Contains("x-environment-id"))
            {
                request.Headers.Remove("x-environment-id");
            }

            request.Headers.Add("x-environment-id", environmentId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}