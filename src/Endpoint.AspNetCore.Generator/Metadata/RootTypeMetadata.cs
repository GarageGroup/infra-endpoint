using System;
using System.Collections.Generic;

namespace GarageGroup.Infra;

internal sealed record class RootTypeMetadata
{
    public RootTypeMetadata(
        string @namespace,
        string typeName,
        DisplayedTypeData providerType,
        IReadOnlyList<string> resolverMethodNames)
    {
        Namespace = @namespace ?? string.Empty;
        TypeName = typeName ?? string.Empty;
        ProviderType = providerType;
        ResolverMethodNames = resolverMethodNames ?? Array.Empty<string>();
    }

    public string Namespace { get; }

    public string TypeName { get; }

    public DisplayedTypeData ProviderType { get; }

    public IReadOnlyList<string> ResolverMethodNames { get; }
}