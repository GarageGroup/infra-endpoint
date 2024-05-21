using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class StringExampleAttribute(string? value) : ExampleAttributeBase
{
    public string? Value { get; } = value;
}