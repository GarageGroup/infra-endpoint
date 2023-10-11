using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class DoubleExampleAttribute : ExampleAttributeBase
{
    public DoubleExampleAttribute(double value)
        =>
        Value = value;

    public double Value { get; }
}