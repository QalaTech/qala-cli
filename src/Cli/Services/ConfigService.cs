using Qala.Cli.Commands.Config;
using Qala.Cli.Models;
using Qala.Cli.Services.Interfaces;
using LanguageExt;

namespace Qala.Cli.Services;

internal class ConfigService(string settingsFilePath) : IConfigService
{
    public async Task<Either<ConfigErrorResponse, ConfigSuccessResponse>> CreateConfigAsync(string key)
    {
        var configFile = $"API_KEY={key}";
        await File.WriteAllTextAsync(settingsFilePath, configFile);

        return await Task.FromResult<Either<ConfigErrorResponse, ConfigSuccessResponse>>(new ConfigSuccessResponse(key));
    }

    public async Task<Config> GetAsync()
    {
        if (File.Exists(settingsFilePath))
        {
            var apiKey = string.Empty;
            var environmentId = string.Empty;
            var fileContent = await File.ReadAllTextAsync(settingsFilePath);
            var lines = fileContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Check which key is being set
                    if (key == "API_KEY")
                    {
                        apiKey = value;
                    }
                    else if (key == "ENVIRONMENT_ID")
                    {
                        environmentId = value;
                    }
                }
            }

            return new Config(apiKey, environmentId);
        }
        else
        {
            throw new Exception("No config file found");
        }
    }
}