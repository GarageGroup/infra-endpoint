using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class SwaggerDescriptionAttribute : Attribute
{
    public SwaggerDescriptionAttribute(string value)
        =>
        Value = value ?? string.Empty;

    public string Value { get; }
}