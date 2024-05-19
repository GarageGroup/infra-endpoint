using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class FloatExampleAttribute(float value) : ExampleAttributeBase
{
    public float Value { get; } = value;
}