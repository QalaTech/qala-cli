namespace Qala.Cli.Utils;

public enum LocalVariableType
{
    QALA_AUTH_TOKEN,
    QALA_API_KEY,
    QALA_ENVIRONMENT_ID
}

public static class Constants
{
    public static readonly Dictionary<LocalVariableType, string> LocalVariable = new()
    {
        { LocalVariableType.QALA_AUTH_TOKEN, "QALA_AUTH_TOKEN" },
        { LocalVariableType.QALA_API_KEY, "QALA_API_KEY" },
        { LocalVariableType.QALA_ENVIRONMENT_ID, "QALA_ENVIRONMENT_ID" }
    };
}
