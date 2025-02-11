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
    public Task<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>> Handle(GenerateMarkdownRequest request, CancellationToken cancellationToken)
    {
        string xmlPath = "qala-cli-xmldoc.xml";
        string markdownPath = "qala-cli-documentation.md";

        GenerateMarkdownFromXml(xmlPath, markdownPath);

        return Task.FromResult<Either<GenerateMarkdownErrorResponse, GenerateMarkdownSuccessResponse>>(new GenerateMarkdownSuccessResponse());
    }

    private static void GenerateMarkdownFromXml(string xmlPath, string markdownPath)
    {
        if (!File.Exists(xmlPath))
        {
            Console.WriteLine("Error: XML file not found!");
            return;
        }

        string xmlContent = File.ReadAllText(xmlPath);
        xmlContent = xmlContent.Trim();
        var doc = XDocument.Parse(xmlContent);
        var sb = new StringBuilder();

        sb.AppendLine("# Qala CLI Documentation");
        sb.AppendLine();

        if (doc.Root != null)
        {
            ProcessCommands(doc.Root, sb, 2);
        }
        else
        {
            Console.WriteLine("Error: XML root element not found!");
        }

        File.WriteAllText(markdownPath, sb.ToString());
        Console.WriteLine($"CLI documentation generated: {markdownPath}");
    }

    private static void ProcessCommands(XElement element, StringBuilder sb, int headingLevel)
    {
        foreach (var command in element.Elements("Command"))
        {
            string commandName = command.Attribute("Name")?.Value ?? "Unknown";
            string description = command.Element("Description")?.Value ?? "No description available.";

            sb.AppendLine($"{new string('#', headingLevel)} `{commandName}`");
            sb.AppendLine();
            sb.AppendLine($"**Description:** {description}");
            sb.AppendLine();

            var parameters = command.Element("Parameters");
            if (parameters != null && parameters.Elements("Option") != null)
            {
                sb.AppendLine("### Options:");
                sb.AppendLine("| Option | Description |");
                sb.AppendLine("|--------|-------------|");

                foreach (var option in parameters.Elements("Option"))
                {
                    string optionTemplate = option.Attribute("Short") != null && !string.IsNullOrWhiteSpace(option.Attribute("Short")?.Value)
                        ? $"`{option.Attribute("Short")?.Value}` / `{option.Attribute("Long")?.Value ?? "Unknown"}`"
                        : $"`{option.Attribute("Long")?.Value}`" ?? "Unknown";
                    string optionDesc = option.Element("Description")?.Value ?? "No description.";

                    sb.AppendLine($"| {optionTemplate} | {optionDesc} |");
                }
                sb.AppendLine();
            }

            var examples = command.Element("Examples");
            if (examples != null)
            {
                sb.AppendLine("### Examples:");
                sb.AppendLine("```sh");
                foreach (var example in examples.Elements("Example"))
                {
                    sb.AppendLine(example.Attribute("commandLine")?.Value ?? "No example provided.");
                }
                sb.AppendLine("```");
                sb.AppendLine();
            }

            ProcessCommands(command, sb, headingLevel + 1);
        }
    }
}