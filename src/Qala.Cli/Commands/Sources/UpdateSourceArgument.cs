using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.Sources;

public class UpdateSourceArgument : CommandSettings
{
    [CommandArgument(0, "<SOURCE_NAME>")]
    [Description("The Name of the source to update.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-n|--name <NEW_NAME>")]
    [Description("The new name of the source.")]
    public string NewName { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the source.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-m|--methods <COMMA_SEPERATED_HTTP_METHODS>")]
    [Description("The comma separated list of the http methods available to the source.")]
    public List<string> Methods { get; set; } = [];

    [CommandOption("-i|--ip-whitelisting <COMMA_SEPERATED_IPS_WHITELISTING>")]
    [Description("The comma separated list of the ips allowed to access the source.")]
    public List<string> IpWhitelisting { get; set; } = [];

    [CommandOption("-a|--authenticationType <AUTHENTICATION_TYPE>")]
    [Description("The authentication type of the source (Basic, ApiKey or JWT).")]
    public string AuthenticationType { get; set; } = string.Empty;

    [CommandOption("--username <USERNAME>")]
    [Description("The username of the source when authentication type is basic.")]
    public string Username { get; set; } = string.Empty;

    [CommandOption("--password <PASSWORD>")]
    [Description("The password of the source when authentication type is basic.")]
    public string Password { get; set; } = string.Empty;

    [CommandOption("--apiKeyName <API_KEY_NAME>")]
    [Description("The name of the api key when authentication type is api key.")]
    public string ApiKeyName { get; set; } = string.Empty;

    [CommandOption("--apiKeyValue <API_KEY_VALUE>")]
    [Description("The value of the api key when authentication type is api key.")]
    public string ApiKeyValue { get; set; } = string.Empty;

    [CommandOption("--issuer <ISSUER>")]
    [Description("The issuer of the source when authentication type is jwt.")]
    public string Issuer { get; set; } = string.Empty;

    [CommandOption("--audience <AUDIENCE>")]
    [Description("The audience of the source when authentication type is jwt.")]
    public string Audience { get; set; } = string.Empty;

    [CommandOption("--algorithm <ALGORITHM>")]
    [Description("The algorithm of the source when authentication type is jwt (RSA or HSA).")]
    public string Algorithm { get; set; } = string.Empty;

    [CommandOption("--publicKey <PUBLIC_KEY>")]
    [Description("The public key of the source when authentication type is jwt and algorithm is RSA.")]
    public string PublicKey { get; set; } = string.Empty;

    [CommandOption("--secret <SECRET>")]
    [Description("The secret of the source when authentication type is jwt and algorithm is HSA.")]
    public string Secret { get; set; } = string.Empty;
}