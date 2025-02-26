using System.Diagnostics;
using System.Runtime.InteropServices;
using Qala.Cli.Data.Repository.Interfaces;

public class LocalEnvironments : ILocalEnvironments
{
    public string GetLocalEnvironment(string variable)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User) ?? string.Empty;
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
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
            return;
        }

        string shellConfigFile = GetShellConfigFile();
        string[] lines = File.ReadAllLines(shellConfigFile);
        bool variableFound = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith($"export {variable}="))
            {
                variableFound = true;
                if (value != null)
                {
                    lines[i] = $"export {variable}={value}";
                }
                else
                {
                    lines[i] = string.Empty; // Mark for removal
                }
            }
        }

        if (!variableFound && value != null)
        {
            Array.Resize(ref lines, lines.Length + 1);
            lines[^1] = $"export {variable}={value}";
        }

        File.WriteAllLines(shellConfigFile, lines.Where(line => !string.IsNullOrWhiteSpace(line)));
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