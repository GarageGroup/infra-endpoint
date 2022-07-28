using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class JsonBodyInAttribute : Attribute
{
    public JsonBodyInAttribute([AllowNull] string propertyName = null)
        =>
        PropertyName = string.IsNullOrEmpty(propertyName) ? null : propertyName;

    public string? PropertyName { get; }
}