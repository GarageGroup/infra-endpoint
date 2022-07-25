using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EndpointTagAttribute : Attribute
{
    public EndpointTagAttribute(string name)
        =>
        Name = name ?? string.Empty;

    public string Name { get; }

    public string? Description { get; set; }
}