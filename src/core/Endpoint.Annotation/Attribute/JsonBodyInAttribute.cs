using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class JsonBodyInAttribute : Attribute
{
    public JsonBodyInAttribute([AllowNull] string propertyName = null)
        =>
        PropertyName = string.IsNullOrEmpty(propertyName) ? null : propertyName;

    public string? PropertyName { get; }
}