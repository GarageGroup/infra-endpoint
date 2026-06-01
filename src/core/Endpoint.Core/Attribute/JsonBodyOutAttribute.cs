using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonBodyOutAttribute([AllowNull] string propertyName = null) : Attribute
{
    public string? PropertyName { get; } = string.IsNullOrEmpty(propertyName) ? null : propertyName;
}