using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HeaderOutAttribute : Attribute
{
    public HeaderOutAttribute([AllowNull] string headerName = null)
        =>
        HeaderName = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? HeaderName { get; }

    public string? Description { get; set; }
}