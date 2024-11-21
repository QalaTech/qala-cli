namespace Qala.Cli.Utils;

public enum EnvironmentVariableType
{
    QALA_AUTH_TOKEN,
    QALA_API_KEY,
    QALA_ENVIRONMENT_ID,
    QALA_MANAGEMENT_API_URL
}

public static class Constants
{
    public static readonly Dictionary<EnvironmentVariableType, string> EnvironmentVariable = new()
    {
        { EnvironmentVariableType.QALA_AUTH_TOKEN, "QALA_AUTH_TOKEN" },
        { EnvironmentVariableType.QALA_API_KEY, "QALA_API_KEY" },
        { EnvironmentVariableType.QALA_ENVIRONMENT_ID, "QALA_ENVIRONMENT_ID" },
        { EnvironmentVariableType.QALA_MANAGEMENT_API_URL, "QALA_MANAGEMENT_API_URL" }
    };
}
