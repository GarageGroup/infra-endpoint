using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class SwaggerDescriptionAttribute(string value) : Attribute
{
    public string Value { get; } = value ?? string.Empty;
}