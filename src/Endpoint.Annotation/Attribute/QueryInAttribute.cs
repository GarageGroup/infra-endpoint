using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryInAttribute([AllowNull] string parameterName = null) : Attribute
{
    public string? ParameterName { get; } = string.IsNullOrEmpty(parameterName) ? null : parameterName;

    public string? Description { get; set; }
}