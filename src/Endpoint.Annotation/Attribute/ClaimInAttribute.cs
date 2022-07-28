using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ClaimInAttribute : Attribute
{
    public ClaimInAttribute([AllowNull] string claimType = null)
        =>
        ClaimType = string.IsNullOrEmpty(claimType) ? null : claimType;

    public string? ClaimType { get; }
}