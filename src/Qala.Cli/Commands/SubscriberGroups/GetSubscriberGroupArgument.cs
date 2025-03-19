using System.ComponentModel;
using Spectre.Console.Cli;

namespace Qala.Cli.Commands.SubscriberGroups;

public class GetSubscriberGroupArgument : CommandSettings
{
    [CommandArgument(0, "<SUBSCRIBER_GROUP_NAME>")]
    [Description("The Name of the subscriber group.")]
    public string Name { get; set; } = string.Empty;
}