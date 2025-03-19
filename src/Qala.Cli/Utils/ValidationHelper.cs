namespace Qala.Cli.Utils;

public static class ValidationHelper
{
    /// <summary>
    /// <para>
    /// Validates audience strings with specific criteria:
    /// <list type="number">
    ///     <item>Length: Must be between 1 and 100 characters.</item>
    ///     <item>Characters Allowed: Can include alphanumeric characters (A-Z, a-z, 0-9).</item>
    ///     <item>Structure: Uses a non-capturing group to define the allowable characters and length: a sequence of 1 to 100
    ///     alphanumeric characters.</item>
    ///     <item>Application: Suitable for validating audience strings in contexts where audience identifiers need to be
    ///     alphanumeric and within a specific length range.</item>
    /// </list>
    /// </para>
    /// </summary>
    public static readonly string AudiencesRegex = @"^(?:[A-Za-z0-9]{1,100})$";
}