namespace GGroupp.Infra;

public sealed record class ProblemData
{
    public ProblemData(string statusFieldName, string? statusCode, string? detail, string? title)
    {
        StatusFieldName = statusFieldName ?? string.Empty;
        StatusCode = statusCode;
        Detail = detail;
        Title = title;
    }

    public string StatusFieldName { get; }

    public string? StatusCode { get; }

    public string? Detail { get; }

    public string? Title { get; }
}