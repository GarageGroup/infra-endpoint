namespace GGroupp.Infra;

internal sealed record class SuccessData
{
    public SuccessData(string? statusCode, string? description)
    {
        StatusCode = statusCode;
        Description = description;
    }

    public string? StatusCode { get; }

    public string? Description { get; }
}