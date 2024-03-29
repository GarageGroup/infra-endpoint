using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
public sealed class SuccessAttribute : Attribute
{
    public SuccessAttribute(SuccessStatusCode statusCode = SuccessStatusCode.Ok)
        =>
        StatusCode = statusCode;

    public SuccessStatusCode StatusCode { get; }

    public string? Description { get; set; }
}