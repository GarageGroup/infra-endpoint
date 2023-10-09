using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryInAttribute : Attribute
{
    public QueryInAttribute([AllowNull] string parameterName = null)
        =>
        ParameterName = string.IsNullOrEmpty(parameterName) ? null : parameterName;

    public string? ParameterName { get; }

    public string? Description { get; set; }
}