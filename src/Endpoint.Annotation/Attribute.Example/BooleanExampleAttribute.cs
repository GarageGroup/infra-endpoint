using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class BooleanExampleAttribute(bool value) : ExampleAttributeBase
{
    public bool Value { get; } = value;
}