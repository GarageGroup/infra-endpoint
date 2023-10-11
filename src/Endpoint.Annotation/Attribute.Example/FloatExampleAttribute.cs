using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class FloatExampleAttribute : ExampleAttributeBase
{
    public FloatExampleAttribute(float value)
        =>
        Value = value;

    public float Value { get; }
}