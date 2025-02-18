using System.Diagnostics;
using System.Runtime.InteropServices;
using Qala.Cli.Data.Repository.Interfaces;

public class LocalEnvironments : ILocalEnvironments
{
    public string GetLocalEnvironment(string variable)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine) ?? string.Empty;
        }

        string? result = null;
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"source {GetShellConfigFile()} && echo ${variable}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            result = proc.StandardOutput.ReadLine();
        }

        proc.WaitForExit();
        return result ?? string.Empty;
    }

    public void SetLocalEnvironment(string variable, string? value)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Machine);
            return;
        }

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"echo 'export {variable}={value}' >> {GetShellConfigFile()}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        proc.Start();
        proc.WaitForExit();
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