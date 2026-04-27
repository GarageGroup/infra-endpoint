using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class DoubleExampleAttribute(double value) : ExampleAttributeBase
{
    public double Value { get; } = value;
}