using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HeaderOutAttribute : Attribute
{
    public HeaderOutAttribute()
        =>
        HeaderName = null;

    public HeaderOutAttribute(string headerName)
        =>
        HeaderName = string.IsNullOrEmpty(headerName) ? null : headerName;

    public string? HeaderName { get; }
}