using System.ComponentModel;
using Qala.Cli.Utils;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class UpdateSubscriberGroupArgument : CommandSettings
{
    [CommandArgument(0, "<SUBSCRIPTION_NAME>")]
    [Description("The name of the subscriber group.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-n|--name <NEW_NAME>")]
    [Description("The new name of the subscriber group.")]
    public string NewName { get; set; } = string.Empty;

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the subscriber group.")]
    public string Description { get; set; } = string.Empty;

    [CommandOption("-t|--topics <TOPICS_COMMA_SEPERATED_NAMES>")]
    [Description("The comma separated list of topic names.")]
    [TypeConverter(typeof(CommaSeparatedStringListConverter))]
    public List<string> Topics { get; set; } = [];

    [CommandOption("-a|--audience <AUDIENCE>")]
    [Description("The audience to scope the subscriber group.")]
    public string Audience { get; set; } = string.Empty;
}