using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryInAttribute : Attribute
{
    public QueryInAttribute([AllowNull] string parameterName = null)
        =>
        ParameterName = string.IsNullOrEmpty(parameterName) ? null : parameterName;

    public string? ParameterName { get; }

    public string? Description { get; set; }
}