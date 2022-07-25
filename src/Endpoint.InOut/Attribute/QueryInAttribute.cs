using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryInAttribute : Attribute
{
    public QueryInAttribute()
        =>
        ParameterName = null;

    public QueryInAttribute(string parameterName)
        =>
        ParameterName = string.IsNullOrEmpty(parameterName) ? null : parameterName;

    public string? ParameterName { get; }
}