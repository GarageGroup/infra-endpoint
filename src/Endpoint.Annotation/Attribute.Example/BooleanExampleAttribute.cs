using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class BooleanExampleAttribute : ExampleAttributeBase
{
    public BooleanExampleAttribute(bool value)
        =>
        Value = value;

    public bool Value { get; }
}