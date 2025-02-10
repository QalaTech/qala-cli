using System.Runtime.InteropServices;
using Qala.Cli.Data.Repository.Interfaces;

public class LocalEnvironments : ILocalEnvironments
{
    public string GetLocalEnvironment(string variable)
    {
        return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process) ?? string.Empty;
    }

    public void SetLocalEnvironment(string variable, string? value)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);

            string shellConfigFile = GetShellConfigFile();
            string exportCommand = $"export {variable}={value}";

            if (!File.ReadAllText(shellConfigFile).Contains(exportCommand))
            {
                File.AppendAllText(shellConfigFile, $"{Environment.NewLine}{exportCommand}");
            }
            else
            {
                Console.WriteLine($"Environment variable '{variable}' is already present in '{shellConfigFile}'.");
            }
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }
    }

    private static string GetShellConfigFile()
    {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string shell = Environment.GetEnvironmentVariable("SHELL") ?? string.Empty;

        if (shell != null && shell.Contains("zsh"))
        {
            return Path.Combine(homeDirectory, ".zshrc");
        }
        else
        {
            return Path.Combine(homeDirectory, ".bash_profile");
        }
    }
}