using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class HeaderInAttribute : Attribute
{
    public HeaderInAttribute([AllowNull] string headerName = null)
        =>
        HeaderName = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? HeaderName { get; }

    public string? Description { get; set; }
}