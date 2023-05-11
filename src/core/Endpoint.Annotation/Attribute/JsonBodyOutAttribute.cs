using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonBodyOutAttribute : Attribute
{
    public JsonBodyOutAttribute([AllowNull] string propertyName = null)
        =>
        PropertyName = string.IsNullOrEmpty(propertyName) ? null : propertyName;

    public string? PropertyName { get; }
}