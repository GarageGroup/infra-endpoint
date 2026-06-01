using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class MediaTypeMetadataAttribute : Attribute
{
    public string? TypeName { get; set; }
}