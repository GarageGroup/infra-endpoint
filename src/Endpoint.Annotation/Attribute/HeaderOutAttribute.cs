using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HeaderOutAttribute([AllowNull] string headerName = null) : Attribute
{
    public string? HeaderName { get; } = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? Description { get; set; }
}