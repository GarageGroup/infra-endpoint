using System.Collections.Generic;
using PrimeFuncPack;

namespace GarageGroup.Infra;

internal sealed record class RootTypeMetadata
{
    public RootTypeMetadata(
        string @namespace,
        string typeName,
        DisplayedTypeData providerType,
        IReadOnlyList<ResolverMethodMetadata> resolverMethods)
    {
        Namespace = @namespace ?? string.Empty;
        TypeName = typeName ?? string.Empty;
        ProviderType = providerType;
        ResolverMethods = resolverMethods ?? [];
    }

    public string Namespace { get; }

    public string TypeName { get; }

    public DisplayedTypeData ProviderType { get; }

    public IReadOnlyList<ResolverMethodMetadata> ResolverMethods { get; }
}