using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class HeaderInAttribute : Attribute
{
    public HeaderInAttribute()
        =>
        HeaderName = null;

    public HeaderInAttribute(string headerName)
        =>
        HeaderName = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? HeaderName { get; }
}