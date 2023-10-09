using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RouteInAttribute : Attribute
{
    public RouteInAttribute([AllowNull] string name = null)
        =>
        Name = string.IsNullOrEmpty(name) ? null : name;

    public string? Name { get; }

    public string? Description { get; set; }
}