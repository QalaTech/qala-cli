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
using Qala.Cli.Data.Gateway.Interfaces;
using Microsoft.Extensions.Configuration;
using Qala.Cli.Gateway;

namespace Qala.Cli.Services;

public class AuthService(IConfiguration configuration, ILocalEnvironments localEnvironments, IAnsiConsole console) : IAuthService
{
    private static readonly HttpClient client = new();

    public async Task<Either<LoginErrorResponse, LoginSuccessResponse>> LoginAsync()
    {
        var authDomain = configuration["Auth:URL"] ?? throw new ArgumentNullException("Auth:URL");
        var authUrl = $"{authDomain}/oauth/device/code";
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", configuration["Auth:ClientID"] ?? throw new ArgumentNullException("Auth:ClientID")),
            new KeyValuePair<string, string>("scope", "openid profile"),
            new KeyValuePair<string, string>("audience", configuration["Auth:Audience"] ?? throw new ArgumentNullException("Auth:Audience")),
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

            console.MarkupLineInterpolated($"Please go to [bold]{deviceCodeResponse.VerificationUriComplete}[/] and enter the code [bold]{deviceCodeResponse.UserCode}[/].");

            Process.Start(new ProcessStartInfo
            {
                FileName = deviceCodeResponse.VerificationUriComplete,
                UseShellExecute = true,
            });

            var token = string.Empty;
            var tokenUrl = $"{authDomain}/oauth/token";
            var tokenContent = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", configuration["Auth:ClientID"] ?? string.Empty),
                new KeyValuePair<string, string>("device_code", deviceCodeResponse.Code),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                new KeyValuePair<string, string>("audience", configuration["Auth:Audience"] ?? string.Empty),
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

            var organizationGateway = new OrganizationGateway(ConfigureHttpClient(configuration, token));
            var organization = await organizationGateway.GetOrganizationAsync();
            if (organization is null)
            {
                return await Task.FromResult<Either<LoginErrorResponse, LoginSuccessResponse>>(new LoginErrorResponse("Organization not found"));
            }

            var environments = organization.Environments.Select(e => new Data.Models.Environment
            {
                Id = e.Id,
                Name = e.Name,
                Region = e.Region,
                EnvironmentType = e.EnvironmentType,
            }).ToList();

            localEnvironments.SetLocalEnvironment(Constants.LocalVariable[LocalVariableType.QALA_AUTH_TOKEN], token);
            
            return new LoginSuccessResponse(token, environments);
        }
        catch (Exception ex)
        {
            return new LoginErrorResponse($"An error occurred: {ex.Message}");
        }
    }

    private static HttpClient ConfigureHttpClient(IConfiguration configuration, string token)
    {
        var httpClient = new HttpClient();
        var baseUrl = configuration["Management-API:URL"] ?? "https://management-api-uat.qalatech.io/";

        httpClient.BaseAddress = new Uri(baseUrl);

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("Token is required.");
        }

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return httpClient;
    }
} 