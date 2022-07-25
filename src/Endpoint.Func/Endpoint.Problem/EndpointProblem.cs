using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra.Endpoint;

public readonly record struct EndpointProblem
{
    private const string AboutBlankTitle = "about:blank";

    public EndpointProblem(
        [AllowNull] string type,
        [AllowNull] string title = AboutBlankTitle,
        FailureStatusCode status = FailureStatusCode.BadRequest,
        [AllowNull] string detail = null)
    {
        Type = string.IsNullOrEmpty(type) ? null : type;
        Title = string.IsNullOrEmpty(title) ? AboutBlankTitle : title;
        Status = status;
        Detail = string.IsNullOrEmpty(detail) ? null : detail;
    }

    public string? Type { get; }

    public string? Title { get; }

    public FailureStatusCode Status { get; }

    public string? Detail { get; }
}