using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class EndpointTagAttribute : Attribute
{
    public EndpointTagAttribute(string name)
        =>
        Name = name ?? string.Empty;

    public string Name { get; }

    public string? Description { get; set; }
}