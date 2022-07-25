using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ProblemAttribute : Attribute
{
    private const string DefaultTitle = "about:blank";

    public ProblemAttribute(FailureStatusCode statusCode, [AllowNull] string detail, [AllowNull] string title = DefaultTitle)
    {
        StatusCode = statusCode;
        Detail = detail ?? string.Empty;
        Title = string.IsNullOrEmpty(title) ? DefaultTitle : title;
    }

    public FailureStatusCode StatusCode { get; }

    public string Detail { get; }

    public string Title { get; }
}