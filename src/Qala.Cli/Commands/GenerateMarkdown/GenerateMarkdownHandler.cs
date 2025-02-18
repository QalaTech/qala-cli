using System.Text;
using System.Xml.Linq;
using LanguageExt;
using MediatR;

namespace Qala.Cli.Commands.GenerateMarkdown;

public record GenerateMarkdownSuccessResponse();
public record GenerateMarkdownErrorResponse(string Message);
public record GenerateMarkdownRequest() : IRequest<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>;

public class GenerateMarkdownHandler : IRequestHandler<GenerateMarkdownRequest, Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>
{
    private const string XmlFileName = "generatedDocs/qala-cli-xmldoc.xml";
    private const string MainMarkdownFile = "generatedDocs/qala-cli-docs.mdx";
    private const string DocsFolder = "generatedDocs";
    private static int sidebarPosition = 2;

    public Task<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>> Handle(GenerateMarkdownRequest request, CancellationToken cancellationToken)
    {
        if (!File.Exists(XmlFileName))
        {
            return Task.FromResult<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>(new GenerateMarkdownErrorResponse("Error: XML file not found!"));
        }

        Directory.CreateDirectory(DocsFolder);
        XDocument doc = XDocument.Load(XmlFileName);

        StringBuilder mainSb = new StringBuilder();
        AddFrontMatter(mainSb, "qala-cli-docs", "Qala CLI Documentation",
            "Qala CLI is a command-line interface that enables you to manage the Q-Flow solution via the terminal. It is suitable for integration into deployment pipelines or for automating certain tasks.",
            ["qala", "q-flow", "documentation", "cli"], sidebarPosition++, false);

        AddHeroBanner(mainSb, "Qala CLI Documentation", "Qala CLI is a command-line interface that enables you to manage the Q-Flow solution via the terminal.");
        mainSb.AppendLine("## Available Commands\n");

        mainSb.AppendLine("<table>");
        mainSb.AppendLine("<tr><th>Command</th><th>Description</th></tr>");

        if (doc.Root != null)
        {
            foreach (var command in doc.Root.Elements("Command"))
            {
                string commandName = command.Attribute("Name")?.Value ?? "Unknown";
                string description = command.Element("Description")?.Value ?? "No description available.";
                var subCommands = command.Elements("Command").ToList();
                string fileName = $"qala-cli-{commandName}.mdx";

                if (subCommands.Count != 0)
                {
                    mainSb.AppendLine($"<tr><td><a href=\"{fileName}\" title=\"Learn about {commandName}\">{commandName}</a></td><td>{description}</td></tr>");
                    GenerateSubCommandMarkdown(command, $"{DocsFolder}/{fileName}", sidebarPosition++);
                }
                else
                {
                    mainSb.AppendLine($"<tr><td><a href=\"#{commandName.ToLower()}\" title=\"Learn about {commandName}\">{commandName}</a></td><td>{description}</td></tr>");
                }
            }

            mainSb.AppendLine("</table>");

            foreach (var command in doc.Root.Elements("Command"))
            {
                var subCommands = command.Elements("Command").ToList();
                if (subCommands.Count == 0)
                {
                    mainSb.AppendLine(GenerateCommandMarkdown(command));
                }
            }

            File.WriteAllText(MainMarkdownFile, mainSb.ToString());
        }
        else
        {
            return Task.FromResult<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>(new GenerateMarkdownErrorResponse("Error: XML root element not found!"));
        }

        return Task.FromResult<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>(new GenerateMarkdownSuccessResponse());
    }

    private static void GenerateSubCommandMarkdown(XElement command, string markdownPath, int position)
    {
        string commandName = command.Attribute("Name")?.Value ?? "Unknown";
        string description = command.Element("Description")?.Value ?? "No description available.";
        var subCommands = command.Elements("Command").ToList();
        string fileName = $"qala-cli-{commandName}";
        List<string> tags = ["qala", "q-flow", "documentation", "cli", commandName];

        var sb = new StringBuilder();
        AddFrontMatter(sb, fileName, commandName, description, tags, position);
        AddHeroBanner(sb, commandName, description);

        sb.AppendLine($"# `{commandName}` Commands\n");

        sb.AppendLine("<table>");
        sb.AppendLine("<tr><th>Command</th><th>Description</th></tr>");

        foreach (var subCommand in subCommands)
        {
            string subCommandName = subCommand.Attribute("Name")?.Value ?? "Unknown";
            string subDescription = subCommand.Element("Description")?.Value ?? "No description available.";
            var deeperSubCommands = subCommand.Elements("Command").ToList();
            string subFileName = $"qala-cli-{subCommandName}.mdx";

            if (deeperSubCommands.Count != 0)
            {
                sb.AppendLine($"<tr><td><a href=\"{subFileName}\" title=\"Learn about {subCommandName}\">{subCommandName}</a></td><td>{subDescription}</td></tr>");
                GenerateSubCommandMarkdown(subCommand, $"{DocsFolder}/{subFileName}", position++);
            }
            else
            {
                sb.AppendLine($"<tr><td><a href=\"#{subCommandName.ToLower()}\" title=\"Learn about {subCommandName}\">{subCommandName}</a></td><td>{subDescription}</td></tr>");
            }
        }

        sb.AppendLine("</table>");

        foreach (var subCommand in subCommands)
        {
            var deeperSubCommands = subCommand.Elements("Command").ToList();
            if (deeperSubCommands.Count == 0)
            {
                sb.AppendLine(GenerateCommandMarkdown(subCommand));
            }
        }

        File.WriteAllText(markdownPath, sb.ToString());
    }

    private static void AddFrontMatter(StringBuilder sb, string id, string title, string description, List<string> tags, int sidebarPosition, bool hideInSidebar = true)
    {
        sb.AppendLine("---");
        sb.AppendLine($"id: {id}");
        sb.AppendLine($"title: {title}");
        sb.AppendLine($"title_meta: {title}");
        sb.AppendLine($"pagination_label: {title}");
        sb.AppendLine($"keywords: [{string.Join(", ", tags)}]");
        sb.AppendLine($"tags: [{string.Join(", ", tags)}]");
        sb.AppendLine($"image: ../docs/img/logo_no_lettering.svg");
        sb.AppendLine($"description: {description}");
        sb.AppendLine($"sidebar_label: {title}");
        sb.AppendLine($"sidebar_position: {sidebarPosition}");
        sb.AppendLine($"draft: true");
        sb.AppendLine($"hide_table_of_contents: false");
        sb.AppendLine($"pagination_next: null");
        sb.AppendLine($"pagination_prev: references/references-intro");
        sb.AppendLine($"hide_title: true");
        sb.AppendLine($"toc_max_heading_level: 2");
        if (hideInSidebar)
        {
            sb.AppendLine("sidebar_class_name: hidden");
        }
        sb.AppendLine("---\n");
    }

    private static string GenerateCommandMarkdown(XElement command)
    {
        var sb = new StringBuilder();
        string commandName = command.Attribute("Name")?.Value ?? "Unknown";
        string description = command.Element("Description")?.Value ?? "No description available.";

        sb.AppendLine($"## `{commandName}`");
        sb.AppendLine($"{description}\n");

        var parameters = command.Element("Parameters");
        if (parameters != null)
        {
            sb.AppendLine("**Options:**");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Option</th><th>Description</th></tr>");

            foreach (var option in parameters.Elements("Option"))
            {
                string optionTemplate = option.Attribute("Short") != null && !string.IsNullOrWhiteSpace(option.Attribute("Short")?.Value)
                    ? $"`{option.Attribute("Short")?.Value}` / `{option.Attribute("Long")?.Value ?? "Unknown"}`"
                    : $"`{option.Attribute("Long")?.Value}`" ?? "Unknown";
                string optionDesc = option.Element("Description")?.Value ?? "No description.";
                sb.AppendLine($"<tr><td>{optionTemplate}</td><td>{optionDesc}</td></tr>");
            }

            sb.AppendLine("</table>");
        }

        var examples = command.Element("Examples");
        if (examples != null)
        {
            sb.AppendLine("**Examples:**");
            sb.AppendLine("```sh");
            foreach (var example in examples.Elements("Example"))
            {
                sb.AppendLine(example.Attribute("commandLine")?.Value ?? "No example provided.");
            }
            sb.AppendLine("```\n");
        }

        return sb.ToString();
    }

    private static void AddHeroBanner(StringBuilder sb, string commandName, string commandDescription)
    {
        sb.AppendLine("import HeroBanner from '../../src/components/HeroBanner/HeroBanner';");
        sb.AppendLine();
        sb.AppendLine($"<HeroBanner");
        sb.AppendLine($"    title=\"{commandName}\"");
        sb.AppendLine($"    tagline=\"{commandDescription}\"");
        sb.AppendLine("    backgroundColor='transparent'");
        sb.AppendLine("/>");
        sb.AppendLine();
    }
}
