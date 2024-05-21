using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class IntegerExampleAttribute(int value) : ExampleAttributeBase
{
    public int Value { get; } = value;
}