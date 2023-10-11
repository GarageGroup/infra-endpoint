using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class IntegerExampleAttribute : ExampleAttributeBase
{
    public IntegerExampleAttribute(int value)
        =>
        Value = value;

    public int Value { get; }
}