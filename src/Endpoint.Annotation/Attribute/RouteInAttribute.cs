using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RouteInAttribute([AllowNull] string name = null) : Attribute
{
    public string? Name { get; } = string.IsNullOrEmpty(name) ? null : name;

    public string? Description { get; set; }
}