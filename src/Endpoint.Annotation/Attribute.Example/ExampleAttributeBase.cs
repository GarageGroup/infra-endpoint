using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public abstract class ExampleAttributeBase : Attribute
{
    private protected ExampleAttributeBase()
    {
    }
}