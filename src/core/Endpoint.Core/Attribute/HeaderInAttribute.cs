using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class HeaderInAttribute([AllowNull] string headerName = null) : Attribute
{
    public string? HeaderName { get; } = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? Description { get; set; }
}