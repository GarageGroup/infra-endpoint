using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class JsonBodyInAttribute([AllowNull] string propertyName = null) : Attribute
{
    public string? PropertyName { get; } = string.IsNullOrEmpty(propertyName) ? null : propertyName;
}