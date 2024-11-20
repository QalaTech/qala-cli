namespace Qala.Cli.Models;

public class ProblemDetails(
    string type,
    string title,
    int status,
    string instance,
    string traceId,
    List<ValidationError> errors)
{
    public string Type { get; set; } = type;
    public string Title { get; set; } = title;
    public int Status { get; set; } = status;
    public string Instance { get; set; } = instance;
    public string TraceId { get; set; } = traceId;
    public List<ValidationError> Errors { get; set; } = errors;
}

public class ValidationError(
    string name,
    string reason,
    string code)
{
    public string Name { get; set; } = name;
    public string Reason { get; set; } = reason;
    public string Code { get; set; } = code;
}
