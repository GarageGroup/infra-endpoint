using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class StringExampleAttribute : ExampleAttributeBase
{
    public StringExampleAttribute(string? value)
        =>
        Value = value;

    public string? Value { get; }
}