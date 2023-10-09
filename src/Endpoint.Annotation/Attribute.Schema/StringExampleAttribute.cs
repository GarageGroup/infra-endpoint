using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class StringExampleAttribute : Attribute
{
    public StringExampleAttribute([AllowNull] string value)
        =>
        Value = value ?? string.Empty;

    public string Value { get; }
}