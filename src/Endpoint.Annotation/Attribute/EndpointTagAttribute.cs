using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class EndpointTagAttribute(string name) : Attribute
{
    public string Name { get; } = name ?? string.Empty;

    public string? Description { get; set; }
}