using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RouteInAttribute : Attribute
{
    public RouteInAttribute()
        =>
        Name = null;

    public RouteInAttribute(string name)
        =>
        Name = string.IsNullOrEmpty(name) ? null : name;

    public string? Name { get; }
}