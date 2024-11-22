using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LanguageExt;
using Qala.Cli.Commands.Login;
using Qala.Cli.Utils;
using Qala.Cli.Data.Models.Auth;
using Qala.Cli.Data.Repository.Interfaces;
using Qala.Cli.Services.Interfaces;
using Spectre.Console;

namespace Qala.Cli.Services;

public class AuthService(ILocalEnvironments localEnvironments) : IAuthService
{
    private static readonly HttpClient client = new();

    public async Task<Either<LoginErrorResponse, LoginSuccessResponse>> LoginAsync()
    {
        var authUrl = "https://dev-lqwdwo2zdrq7ygsg.us.auth0.com/oauth/device/code";
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", "gv2q0aAxsQ1mezQtZ4ao9DJjGOYFeybB"),
            new KeyValuePair<string, string>("scope", "openid profile"),
            new KeyValuePair<string, string>("audience", "https://localhost:7143/"),
        ]);

        try
        {
            var response = await client.PostAsync(authUrl, content);
            response.EnsureSuccessStatusCode();
            var deviceCodeResponse = await response.Content.ReadFromJsonAsync<DeviceCode>();
            if (deviceCodeResponse is null)
            {
                return new LoginErrorResponse("Failed to retrieve device code.");
            }

            AnsiConsole.MarkupLineInterpolated($"Please go to [bold]{deviceCodeResponse.VerificationUriComplete}[/] and enter the code [bold]{deviceCodeResponse.UserCode}[/].");

            Process.Start(new ProcessStartInfo
            {
                FileName = deviceCodeResponse.VerificationUriComplete,
                UseShellExecute = true,
            });

            var token = string.Empty;
            var tokenUrl = "https://dev-lqwdwo2zdrq7ygsg.us.auth0.com/oauth/token";
            var tokenContent = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", "gv2q0aAxsQ1mezQtZ4ao9DJjGOYFeybB"),
                new KeyValuePair<string, string>("device_code", deviceCodeResponse.Code),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                new KeyValuePair<string, string>("audience", "https://localhost:7143/"),
            ]);

            while (string.IsNullOrEmpty(token))
            {
                await Task.Delay(deviceCodeResponse.Interval * 1000);

                var tokenResponse = await client.PostAsync(tokenUrl, tokenContent);

                if (tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResponseData = await tokenResponse.Content.ReadFromJsonAsync<AuthToken>(
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        }
                    );
                    if (tokenResponseData != null)
                    {
                        token = tokenResponseData.AccessToken;
                    }
                    else
                    {
                        return new LoginErrorResponse("Failed to retrieve token response data.");
                    }
                }
                else if (tokenResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    var errorResponseData = await tokenResponse.Content.ReadFromJsonAsync<AuthToken>();
                    if (errorResponseData != null && errorResponseData.Error == "authorization_pending")
                    {
                        continue;
                    }
                    else if (errorResponseData != null && errorResponseData.Error == "slow_down")
                    {
                        deviceCodeResponse.Interval = int.Parse(errorResponseData.Interval) * 1000;
                        continue;
                    }
                    else
                    {
                        return new LoginErrorResponse(errorResponseData?.ErrorDescription ?? "Unknown error occurred.");
                    }
                }
                else
                {
                    return new LoginErrorResponse("An error occurred while retrieving the token.");
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                return new LoginErrorResponse("Failed to retrieve token.");
            }

            localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], token);
            
            return new LoginSuccessResponse(token);
        }
        catch (Exception ex)
        {
            return new LoginErrorResponse($"An error occurred: {ex.Message}");
        }
    }
} 