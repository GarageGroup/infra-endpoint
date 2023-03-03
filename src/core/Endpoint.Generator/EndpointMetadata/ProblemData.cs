namespace GGroupp.Infra;

internal sealed record class ProblemData
{
    public ProblemData(string statusFieldName, string? statusCode, string? detail, string? title, string? description)
    {
        StatusFieldName = statusFieldName ?? string.Empty;
        StatusCode = statusCode;
        Detail = detail;
        Title = title;
        Description = description;
    }

    public string StatusFieldName { get; }

    public string? StatusCode { get; }

    public string? Detail { get; }

    public string? Title { get; }

    public string? Description { get; }
}