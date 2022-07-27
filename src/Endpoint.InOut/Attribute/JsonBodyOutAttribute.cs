using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonBodyOutAttribute : Attribute
{
    public JsonBodyOutAttribute([AllowNull] string propertyName = null)
        =>
        PropertyName = string.IsNullOrEmpty(propertyName) ? null : propertyName;

    public string? PropertyName { get; }
}