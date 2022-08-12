using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class StringExampleAttribute : Attribute
{
    public StringExampleAttribute([AllowNull] string value)
        =>
        Value = value ?? string.Empty;

    public string Value { get; }
}